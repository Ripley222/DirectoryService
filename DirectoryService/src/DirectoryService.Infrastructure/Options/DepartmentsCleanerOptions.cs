namespace DirectoryService.Infrastructure.Options;

public class DepartmentsCleanerOptions
{
    public const string SectionName = "DepartmentsCleaner";
    
    public int PeriodOfTimeInHours { get; init; }
}