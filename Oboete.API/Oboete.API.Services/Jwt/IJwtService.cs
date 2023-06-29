using LanguageExt;
using Oboete.API.Entities.Users;
using Oboete.API.Services.Errors;

namespace Oboete.API.Services.Jwt;

public interface IJwtService
{
    Task<Either<ApplicationError, JwtTokenIssuance>> IssueTokenAsync(ApplicationUser user);
    Task<Either<ApplicationError, JwtTokenIssuance>> RefreshTokenAsync(string refreshToken);
}