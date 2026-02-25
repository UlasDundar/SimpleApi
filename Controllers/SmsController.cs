using Microsoft.AspNetCore.Mvc;
using SimpleApi.Models;
using SimpleApi.Services;
using SimpleApi.Data; // DbContext için
using Microsoft.EntityFrameworkCore;

namespace SimpleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;
    private readonly ILogger<SmsController> _logger;
    private readonly ApplicationDbContext _db;

    public SmsController(ISmsService smsService, ILogger<SmsController> logger, ApplicationDbContext db)
    {
        _smsService = smsService;
        _logger = logger;
        _db = db;
    }

    // POST /api/sms/send
    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SendSmsRequest request)
    {
        _logger.LogInformation("SMS isteği alındı. Numara: {Phone}", request.Phone);

        var result = _smsService.Send(request.Phone, request.Message);

        var entity = new SmsEntity
        {
            Phone = request.Phone,
            Message = request.Message,
            SentAt = DateTime.UtcNow
        };

        _db.SmsMessages.Add(entity);
        await _db.SaveChangesAsync();

        var response = new ApiResponse<string>
        {
            Success = true,
            Message = "SMS başarıyla gönderildi",
            Data = result
        };

        return Ok(response);
    }

    // GET /api/sms
    [HttpGet]
    public async Task<IActionResult> GetAllSms()
    {
        var messages = await _db.SmsMessages.ToListAsync();
        var response = new ApiResponse<IEnumerable<SmsEntity>>
        {
            Success = true,
            Message = "Tüm SMS mesajları listelendi",
            Data = messages
        };
        return Ok(response);
    }

    // GET /api/sms/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSmsById(int id)
    {
        var message = await _db.SmsMessages.FindAsync(id);
        if (message == null)
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "SMS bulunamadı",
                Data = null
            });

        var response = new ApiResponse<SmsEntity>
        {
            Success = true,
            Message = "SMS bulundu",
            Data = message
        };
        return Ok(response);
    }

    // DELETE /api/sms/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSms(int id)
    {
        var entity = await _db.SmsMessages.FindAsync(id);
        if (entity == null)
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "SMS bulunamadı",
                Data = null
            });

        _db.SmsMessages.Remove(entity);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "SMS silindi",
            Data = null
        });
    }
}