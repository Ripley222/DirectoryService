namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsDto(
    IEnumerable<DepartmentDto> Departments);