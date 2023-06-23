using Microsoft.AspNetCore.Identity;
using Oboete.API.Entities.Users;

namespace Oboete.API.BackgroundServices;

public class UserRolesBackgroundService : BackgroundService
{
    private readonly ILogger<UserRolesBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public static bool TaskCompleted { get; private set; }

    public UserRolesBackgroundService(ILogger<UserRolesBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!EFCoreMigrationsBackgroundService.TaskCompleted)
        {
            await Task.Delay(100, stoppingToken);
        }

        _logger.LogInformation("Starting User Roles Task");
        await EnsureRolesAreCreatedAsync();
        _logger.LogInformation("Finishing User Roles Task");
    }

    private async Task EnsureRolesAreCreatedAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        _logger.LogInformation("Starting User Roles Task");
        foreach (var role in UserRoles.AllRoles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        _logger.LogInformation("Finishing User Roles Task");
        TaskCompleted = true;
    }
}