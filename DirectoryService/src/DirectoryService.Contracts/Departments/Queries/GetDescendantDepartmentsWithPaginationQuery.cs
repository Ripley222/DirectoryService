namespace DirectoryService.Contracts.Departments.Queries;

public record GetDescendantDepartmentsWithPaginationQuery(
    Guid DepartmentId,
    int Page = 1,
    int Size = 20);