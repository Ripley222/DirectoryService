using DirectoryService.Application.DistributedCaching;

namespace DirectoryService.Infrastructure.Options;

public class CacheOptions : ICacheOptions
{
    public const string SECTION_NAME = "CachingData";
    
    public int TimeToClearInMinutes { get; init; }
}