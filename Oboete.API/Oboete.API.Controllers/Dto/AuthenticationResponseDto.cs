namespace Oboete.API.Controllers.Dto;

public class AuthenticationResponseDto
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime Expiration { get; init; }
}