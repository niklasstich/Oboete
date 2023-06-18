using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oboete.API.Entities;

namespace Oboete.API.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly int _expirationMinutes;
    private readonly int _refreshTokenExpirationDays;
    private readonly string _jwtKey;
    private readonly string _jwtSubject;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager,
        JwtSecurityTokenHandler tokenHandler)
    {
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        if (!int.TryParse(configuration["Jwt:TokenValidityInMinutes"], out _expirationMinutes))
            _expirationMinutes = 1;
        if (!int.TryParse(configuration["Jwt:RefreshTokenValidityInDays"], out _refreshTokenExpirationDays))
            _refreshTokenExpirationDays = 7;
        _jwtKey = configuration["Jwt:Key"] ??
                  throw new ApplicationException("No 'Jwt:Key' found in configuration");
        _jwtSubject = configuration["Jwt:Subject"] ??
                      throw new ApplicationException("No 'Jwt:Subject' found in configuration");
        _jwtIssuer = configuration["Jwt:Issuer"] ??
                     throw new ApplicationException("No 'Jwt:Issuer' found in configuration");
        _jwtAudience = configuration["Jwt:Audience"] ??
                       throw new ApplicationException("No 'Jwt:Audience' found in configuration");
    }

    public async Task<JwtTokenIssuance> IssueTokenAsync(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_expirationMinutes);

        var token = CreateJwtToken(
            await CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        await _userManager.UpdateAsync(user);

        return new JwtTokenIssuance
        {
            Token = tokenHandler.WriteToken(token),
            Expiration = expiration,
            RefreshToken = refreshToken
        };
    }

    public async Task<JwtTokenIssuance> RefreshTokenAsync(string refreshToken)
    {
        //check if user and refesh token are valid and not expired yet
        var user = await _userManager.Users.FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            throw new SecurityTokenException("Invalid or expired refresh token");

        return await IssueTokenAsync(user);
    }

    private string GenerateRefreshToken()
    {
        var randomData = new Span<byte>(new byte[64]);
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomData);
        return Convert.ToBase64String(randomData);
    }

    private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials,
        DateTime expiration)
    {
        return new JwtSecurityToken(
            _jwtIssuer,
            _jwtAudience,
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
    }

    private async Task<IEnumerable<Claim>> CreateClaims(ApplicationUser user)
    {
        if (user.UserName is null)
            throw new ArgumentException("User must have a username", nameof(user));
        if (user.Email is null)
            throw new ArgumentException("User must have an email", nameof(user));
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, _jwtSubject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SigningCredentials CreateSigningCredentials() =>
        new(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
}