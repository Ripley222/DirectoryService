namespace DirectoryService.Infrastructure.Options;

public class CacheOptions() 
{
    public const string SECTION_NAME = "CachingData";
    public int TimeToClearInMinutes { get; init; }
}