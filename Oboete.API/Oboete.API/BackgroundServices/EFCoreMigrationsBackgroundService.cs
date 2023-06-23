using Microsoft.EntityFrameworkCore;
using Oboete.API.Entities.Users;
using Timer = System.Timers.Timer;

namespace Oboete.API.BackgroundServices;

public class EFCoreMigrationsBackgroundService : BackgroundService
{
    private const int DbTimeoutInSeconds = 30;
    
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
        //why service locator?
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var dbTimeoutHit = false;
        using var timer = new Timer(DbTimeoutInSeconds * 1000);
        timer.Elapsed += (_, _) =>
        {
            dbTimeoutHit = true;
        };
        timer.AutoReset = false;
        timer.Start();
        while (!await dbContext.Database.CanConnectAsync(stoppingToken))
        {
            await Task.Delay(1000, stoppingToken);
            if (!dbTimeoutHit) continue;
            _logger.LogCritical("Database connection timed out after {DbTimeoutInSeconds} seconds", DbTimeoutInSeconds);
            throw new TimeoutException($"Database connection timed out after {DbTimeoutInSeconds} seconds");
        }
        timer.Stop();

        await dbContext.Database.MigrateAsync(stoppingToken);
        _logger.LogInformation("Finishing EF Core Migrations Task");
        TaskCompleted = true;
    }
}