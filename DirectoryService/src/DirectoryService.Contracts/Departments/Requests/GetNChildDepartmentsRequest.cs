namespace DirectoryService.Contracts.Departments.Requests;

public record GetNChildDepartmentsRequest(
    int Page = 1,
    int Size = 20,
    int Prefetch = 3);