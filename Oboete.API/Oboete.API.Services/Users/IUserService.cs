using LanguageExt;
using Oboete.API.Entities.Users;
using Oboete.API.Services.Errors;

namespace Oboete.API.Services.Users;

public interface IUserService
{
    Task<Either<ApplicationError, ApplicationUser>> RegisterUserAsync(string username, string email, string password);
    Task<Either<ApplicationError, ApplicationUser>> CheckCredentialsAsync(string username, string password);
}