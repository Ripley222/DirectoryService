namespace DirectoryService.Contracts.Departments.Requests;

public record GetDepartmentsWithPaginationRequest(
    int Page = 1,
    int Size = 20);