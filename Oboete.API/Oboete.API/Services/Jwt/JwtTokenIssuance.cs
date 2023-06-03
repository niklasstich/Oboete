namespace Oboete.API.Services.Jwt;

public struct JwtTokenIssuance
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime Expiration { get; init; }
}