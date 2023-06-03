using Oboete.API.Entities;

namespace Oboete.API.Services.Users;

public interface IUserService
{
    Task<ApplicationUser> RegisterUserAsync(string username, string email, string password);
    Task<ApplicationUser> CheckCredentialsAsync(string username, string password);
}