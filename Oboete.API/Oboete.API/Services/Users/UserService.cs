using Microsoft.AspNetCore.Identity;
using Oboete.API.Entities;

namespace Oboete.API.Services.Users;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public UserService(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser> RegisterUserAsync(string username, string email, string password)
    {
        var applicationUser = new ApplicationUser { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(applicationUser, password);
        
        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(error => error.Description)));
        }
        
        await _userManager.AddToRoleAsync(applicationUser, UserRoles.User);

        return applicationUser;
    }

    public async Task<ApplicationUser> CheckCredentialsAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            throw new ArgumentException("Invalid credentials");
        return user;
    }
}