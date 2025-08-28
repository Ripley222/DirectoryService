using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Repositories;

public interface IDepartmentsRepository
{
    Task<Result<Department, Error>> GetById(DepartmentId departmentId, CancellationToken cancellationToken);
    
    Task<Result<IReadOnlyList<Department>, Error>> GetManyById(
        IEnumerable<DepartmentId> departmentIds, CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> Add(Department department, CancellationToken cancellationToken);
    
    Task<Result<Department, Error>> GetByIdentifier(Identifier identifier, CancellationToken cancellationToken);
}