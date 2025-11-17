using DirectoryService.Application.DistributedCaching;
using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Core.Abstractions.Caching;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class DepartmentsCleanerBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<DepartmentsCleanerOptions> options,
    ILogger<DepartmentsCleanerBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Departments cleaner background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWork(stoppingToken);

            var departmentsCleanerOptions = options.Value;

            var delay = TimeSpan.FromHours(departmentsCleanerOptions.PeriodOfTimeInHours);

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        logger.LogInformation("Departments cleaner background service is working.");

        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var departmentRepository = scope.ServiceProvider.GetRequiredService<IDepartmentsRepository>();
        
        var result = await departmentRepository.RemoveDeactivatedDepartments(cancellationToken);
        if (result > 0)
        {
            //инвалидация кэша
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            await cacheService.RemoveByPrefixAsync(CacheConstants.CACHING_DEPARTMENTS_KEY, cancellationToken);
        }
    }
}