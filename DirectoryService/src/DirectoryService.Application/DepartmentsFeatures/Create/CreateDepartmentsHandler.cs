using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Application.DepartmentsFeatures.Create;

public class CreateDepartmentsHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    IValidator<CreateDepartmentsCommand> validator,
    ILogger<CreateDepartmentsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateDepartmentsCommand command,
        CancellationToken cancellationToken = default)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();
        
        var departmentId = DepartmentId.New();
        var departmentName = DepartmentName.Create(command.Name).Value;
        var identifier = Identifier.Create(command.Identifier).Value;
        
        //бизнес валидация
        //проверка на существование локаций
        var locationsExistResult = await locationsRepository.CheckManyByIds(
            command.LocationIds.Select(LocationId.Create), cancellationToken);

        if (locationsExistResult.IsFailure)
            return locationsExistResult.Error.ToErrors();


        //проверка на уникальность identifier
        var departmentExistWithIdentifier = await departmentsRepository.CheckByIdentifier(
            identifier, cancellationToken);

        if (departmentExistWithIdentifier.IsSuccess)
            return Errors.Department.AlreadyExist("Identifier").ToErrors();


        //проверка на наличие родительского департамента
        Department? department = null;
        if (command.ParentId is not null)
        {
            var parentDepartmentResult = await departmentsRepository.GetById(
                DepartmentId.Create((Guid)command.ParentId), cancellationToken);
            
            if (parentDepartmentResult.IsFailure)
                return parentDepartmentResult.Error.ToErrors();
            
            department = parentDepartmentResult.Value;
        }
        
        var path = department is not null
            ? Path.Create(department.Path.Value + "." + identifier.Value).Value
            : Path.Create(command.Identifier).Value;

        var depth = department is not null
            ? (short)(department.Depth + Department.CHILD_DEPARTMENT_DEPTH)
            : (short)Department.MAIN_DEPARTMENT_DEPTH;
    
        //создание нового департамента
        var departmentResult = Department.Create(
            departmentId,
            departmentName,
            identifier,
            path,
            depth,
            department);

        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        //привязка департамента к локациям
        List<DepartmentLocation> departmentLocations = [];
        foreach (var locationId in command.LocationIds)
        {
            var departmentsLocations = new DepartmentLocation(
                departmentId,
                LocationId.Create(locationId));
            
            departmentLocations.Add(departmentsLocations);
        }
        
        departmentResult.Value.AddLocations(departmentLocations);
        
        //добавление нового департамента
        var result = await departmentsRepository.Add(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        logger.LogInformation("Created new Department with id {id}", departmentId.Value);
        
        return departmentId.Value;
    }
}