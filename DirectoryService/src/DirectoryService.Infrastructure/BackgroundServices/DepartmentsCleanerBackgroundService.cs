﻿using DirectoryService.Application.Repositories;
using DirectoryService.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        await departmentRepository.RemoveDeactivatedDepartments(cancellationToken);
    }
}