using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class DepartmentsCleanerBackgroundService(
    IConfiguration configuration,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DepartmentsCleanerBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Departments cleaner background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWork(stoppingToken);
            
            var options = configuration
                .GetSection(DepartmentsCleanerOptions.SectionName)
                .Get<DepartmentsCleanerOptions>()
                ?? throw new ApplicationException("Missing departments cleaner background service options.");
                
            var delay = TimeSpan.FromHours(options.PeriodOfTime);
            
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        logger.LogInformation("Departments cleaner background service is working.");
        
        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var departmentRepository = scope.ServiceProvider.GetRequiredService<IDepartmentsRepository>();

        await departmentRepository.RemoveDeactivatedDepartments(cancellationToken);
    }
}