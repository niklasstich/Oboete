using Microsoft.AspNetCore.Identity;
using Oboete.API.Entities.Users;

namespace Oboete.API.BackgroundServices;

public class AdminUserBackgroundService : BackgroundService
{
    private readonly ILogger<AdminUserBackgroundService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public AdminUserBackgroundService(ILogger<AdminUserBackgroundService> logger, IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //ensure roles are created before we create the admin user
        while (!UserRolesBackgroundService.TaskCompleted)
            await Task.Delay(100, stoppingToken);
        _logger.LogInformation("Starting Admin User Task");
        await EnsureDefaultAdminUserIsCreatedAsync();
        _logger.LogInformation("Finishing Admin User Task");
    }

    private async Task EnsureDefaultAdminUserIsCreatedAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var password = _configuration["DefaultAdminPassword"] ??
                       throw new ApplicationException("No 'DefaultAdminPassword' found in configuration");

        //admin already exists
        var existingAdminUser = await userManager.FindByNameAsync("admin");
        if (existingAdminUser != null)
        {
            //check if password is correct, if not, change it
            if (await userManager.CheckPasswordAsync(existingAdminUser, password)) return;
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(existingAdminUser);
            await userManager.ResetPasswordAsync(existingAdminUser, resetToken, password);
            return;
        }

        var adminUser = new ApplicationUser { UserName = "admin", Email = "admin@example.com" };
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ApplicationException("You must set 'DefaultAdminPassword' in configuration");
        }

        await userManager.CreateAsync(adminUser, password);
        await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
    }
}