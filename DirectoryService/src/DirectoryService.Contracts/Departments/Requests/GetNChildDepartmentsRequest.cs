namespace DirectoryService.Contracts.Departments.Requests;

public record GetNChildDepartmentsRequest(
    GetDepartmentsWithPaginationRequest GetDepartmentsWithPagination,
    int Prefetch = 3);