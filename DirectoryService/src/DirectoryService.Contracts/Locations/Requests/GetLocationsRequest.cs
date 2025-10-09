namespace DirectoryService.Contracts.Locations.Requests;

public record GetLocationsRequest(
    IEnumerable<Guid>? DepartmentsIds,
    string? Search,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "date",
    string? SortDirection = "asc");