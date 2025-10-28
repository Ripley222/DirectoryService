using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.DTOs;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Application.Repositories;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> Add(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetById(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetByIdWithLocations(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetByIdWithLock(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<UnitResult<Error>> CheckActiveDepartmentsByIds(IEnumerable<DepartmentId> departmentIds, CancellationToken cancellationToken);
    Task<UnitResult<Error>> CheckByIdentifier(Identifier identifier, CancellationToken cancellationToken);
    Task<UnitResult<Error>> IsDescendants(DepartmentId rootDepartmentId, DepartmentId candidateChildDepartmentId);
    Task<UnitResult<Error>> LockDescendants(Path parentPath);
    Task<UnitResult<Error>> UpdateDescendantDepartments(Department department, Path oldPath);
    Task<UnitResult<Error>>UpdateRelationships(DepartmentId departmentId);
}