using Oboete.API.Entities;

namespace Oboete.API.Services.Jwt;

public interface IJwtService
{
    Task<JwtTokenIssuance> IssueTokenAsync(ApplicationUser user);
    Task<JwtTokenIssuance> RefreshTokenAsync(string refreshToken);
}