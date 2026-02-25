using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Models;
using SimpleApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using SimpleApi.Services; // CacheService burada varsayılıyor

namespace SimpleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly CacheService _cacheService;

    public AuthController(
        ApplicationDbContext context,
        IConfiguration configuration,
        CacheService cacheService)
    {
        _context = context;
        _configuration = configuration;
        _cacheService = cacheService;
    }

    // ================= REGISTER =================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            return BadRequest("Bu email zaten kayıtlı.");

        var user = new User
        {
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Kayıt başarılı.");
    }

    // ================= LOGIN =================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return Unauthorized("Email veya şifre hatalı.");

        var validPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

        if (!validPassword)
            return Unauthorized("Email veya şifre hatalı.");

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    // ================= SECURE =================

    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok("Token doğruysa burayı görürsün.");
    }

    // ================= ADMIN =================

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Sadece Admin.");
    }

    // ================= REDIS CACHE TEST =================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var cacheKey = $"user:{id}";

        var cachedUser = await _cacheService.GetAsync<User>(cacheKey);

        if (cachedUser != null)
        {
            return Ok(new
            {
                source = "redis",
                data = cachedUser
            });
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        await _cacheService.SetAsync(cacheKey, user);

        return Ok(new
        {
            source = "database",
            data = user
        });
    }

    // ================= JWT =================

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] string newEmail)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.Email = newEmail;

        await _context.SaveChangesAsync();

        // 🔥 CACHE INVALIDATION
        var cacheKey = $"user:{id}";
        await _cacheService.RemoveAsync(cacheKey);

        return Ok(new
        {
            message = "User güncellendi ve cache temizlendi."
        });
    }
}