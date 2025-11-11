namespace DirectoryService.Application.DistributedCaching;

public interface ICacheOptions
{
    int TimeToClearInMinutes { get; init; }
}