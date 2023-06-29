using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Oboete.API.Entities.Users;
using Oboete.API.Services.Errors;
using Oboete.API.Services.Errors.Users;

namespace Oboete.API.Services.Users;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(IConfiguration configuration, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Either<ApplicationError, ApplicationUser>> RegisterUserAsync(string username, string email,
        string password)
    {
        var applicationUser = new ApplicationUser { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(applicationUser, password);

        if (!result.Succeeded)
        {
            return MapIdentityError(result.Errors.First());
        }

        await _userManager.AddToRoleAsync(applicationUser, UserRoles.User);

        return applicationUser;
    }

    private ApplicationError MapIdentityError(IdentityError error)
    {
        switch (error.Code)
        {
            case nameof(IdentityErrorDescriber.InvalidEmail):
            case nameof(IdentityErrorDescriber.InvalidUserName):
            case nameof(IdentityErrorDescriber.PasswordTooShort):
            case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
            case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
            case nameof(IdentityErrorDescriber.PasswordRequiresLower):
            case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
            case nameof(IdentityErrorDescriber.DuplicateEmail):
            case nameof(IdentityErrorDescriber.DuplicateUserName):
                return new RegisterInvalidInputError(error.Description);
            default:
                return new RegisterExceptionalError("Internal error registering user");
        }
    }


    public async Task<Either<ApplicationError, ApplicationUser>> CheckCredentialsAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return new InvalidCredentialsError("Invalid credentials");
        return user;
    }
}