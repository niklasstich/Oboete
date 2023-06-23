using Microsoft.AspNetCore.Identity;

namespace Oboete.API.Entities.Users;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}