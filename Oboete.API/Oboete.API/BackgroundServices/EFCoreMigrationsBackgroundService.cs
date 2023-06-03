using Microsoft.EntityFrameworkCore;
using Oboete.API.Entities;

namespace Oboete.API.BackgroundServices;

public class EFCoreMigrationsBackgroundService : BackgroundService
{
    private readonly ILogger<EFCoreMigrationsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public static bool TaskCompleted { get; private set; }

    public EFCoreMigrationsBackgroundService(ILogger<EFCoreMigrationsBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting EF Core Migrations Task");
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        while (!await dbContext.Database.CanConnectAsync(stoppingToken))
        {
        }

        await dbContext.Database.MigrateAsync(stoppingToken);
        _logger.LogInformation("Finishing EF Core Migrations Task");
        TaskCompleted = true;
    }
}