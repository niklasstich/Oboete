using Microsoft.AspNetCore.Identity;

namespace Oboete.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}