using SimpleApi.Models;

namespace SimpleApi.Services;

public interface IUserService
{
    Task<User> RegisterAsync(string username, string email, string password, string role = "User");
    Task<string?> LoginAsync(string username, string password);
}