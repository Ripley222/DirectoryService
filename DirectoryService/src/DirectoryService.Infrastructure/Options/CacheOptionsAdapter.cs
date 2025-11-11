using DirectoryService.Application.DistributedCaching;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Options;

public class CacheOptionsAdapter(IOptions<CacheOptions> options) : ICacheOptions
{
    public int TimeToClearInMinutes { get; init; } = options.Value.TimeToClearInMinutes;
}