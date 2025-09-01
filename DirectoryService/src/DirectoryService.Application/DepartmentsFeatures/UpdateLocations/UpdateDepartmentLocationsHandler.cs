using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.UpdateLocations;

public class UpdateDepartmentLocationsHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    IValidator<UpdateDepartmentLocationsCommand> validator)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken = default)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();
        
        var departmentId = DepartmentId.Create(command.DepartmentId);
        
        //бизнес валидация
        //открытие транзакции
        var transactionScopeResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();
        
        using var transactionScope = transactionScopeResult.Value;

        //получение департамента и проверка на то, что он активен
        var department = await departmentsRepository.GetById(departmentId, cancellationToken);
        if (department.IsFailure)
        {
            transactionScope.Rollback();
            return department.Error.ToErrors();
        }

        if (department.Value.IsActive() is false)
        {
            transactionScope.Rollback();
            return Errors.Department.NotActive().ToErrors();
        }

        //проверка на существование и активность локаций
        var activeLocationsResult = await locationsRepository.CheckActiveLocationsByIds(
            command.LocationIds.Select(LocationId.Create), cancellationToken);

        if (activeLocationsResult.IsFailure)
        {
            transactionScope.Rollback();
            return activeLocationsResult.Error.ToErrors();
        }

        //привязка локаций к департаменту
        List<DepartmentLocation> departmentLocations = [];
        foreach (var locationId in command.LocationIds)
        {
            var departmentLocation = new DepartmentLocation(
                departmentId,
                LocationId.Create(locationId));
            
            departmentLocations.Add(departmentLocation);
        }
        
        department.Value.AddLocations(departmentLocations);
        
        //удаление прошлых локаций
        await departmentsRepository.DeleteLocationsByDepartmentId(departmentId, cancellationToken);
        
        //сохранение изменений
        var result = await transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            transactionScope.Rollback();
            return result.Error.ToErrors();
        }
        
        //применение изменений
        var commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        return departmentId.Value;
    }
}