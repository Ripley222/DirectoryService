namespace DirectoryService.Contracts.Departments.Queries;

public record GetNChildDepartmentsQuery(
    int Page = 1,
    int Size = 20,
    int Prefetch = 3);