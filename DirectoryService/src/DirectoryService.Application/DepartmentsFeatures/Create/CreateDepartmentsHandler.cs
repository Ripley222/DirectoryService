using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
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
        
        //бизнес валидация
        //проверка на уникальность identifier
        var departmentWithIdentifierExits = await CheckExistenceDepartmentByIdentifier(
            Identifier.Create(command.Identifier).Value, cancellationToken);
        
        if (departmentWithIdentifierExits.IsFailure)
            return departmentWithIdentifierExits.Error.ToErrors();
        
        //проверка на наличие родителя
        if (command.ParentId is null)
        {
            //создание корневого департамента
            var mainDepartmentResult = await CreateMainDepartment(command, cancellationToken);
            if (mainDepartmentResult.IsFailure)
                return mainDepartmentResult.Error.ToErrors();
            
            logger.LogInformation("Creating main department with id {id}", mainDepartmentResult.Value);
            
            return mainDepartmentResult.Value;
        }
        else
        {
            //получения родительского департамента
            var parentDepartmentResult = await departmentsRepository
                .GetById(DepartmentId.Create((Guid)command.ParentId), cancellationToken);

            if (parentDepartmentResult.IsFailure)
                return parentDepartmentResult.Error.ToErrors();
            
            //создание дочернего депортамента
            var childDepartmentResult = await CreateChildDepartment(
                parentDepartmentResult.Value,
                command, 
                cancellationToken);
            
            if (childDepartmentResult.IsFailure)
                return childDepartmentResult.Error.ToErrors();
            
            logger.LogInformation("Creating child department with id {id}", childDepartmentResult.Value);

            return childDepartmentResult.Value;
        }
    }

    private async Task<Result<Guid, Error>> CreateMainDepartment(
        CreateDepartmentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var departmentId = DepartmentId.New();
        var departmentName = DepartmentName.Create(command.Name).Value;
        var identifier = Identifier.Create(command.Identifier).Value;
        var path = Path.Create(command.Identifier).Value;
        short depth = Department.MAIN_DEPARTMENT_DEPTH;

        var departmentResult = Department.Create(
            departmentId,
            departmentName,
            identifier,
            path,
            depth);

        if (departmentResult.IsFailure)
            return departmentResult.Error;
        
        //закрелпение департамента за локацией
        var addedDepartmentResult = await AddDepartmentToLocation(
            departmentResult.Value,
            command.LocationIds,
            cancellationToken);
        
        if (addedDepartmentResult.IsFailure)
            return addedDepartmentResult.Error;
        
        //сохранения родительского департамента в БД
        var result = await departmentsRepository.Add(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return departmentResult.Value.Id.Value;
    }

    private async Task<Result<Guid, Error>> CreateChildDepartment(
        Department parentDepartment,
        CreateDepartmentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var departmentId = DepartmentId.New();
        var departmentName = DepartmentName.Create(command.Name).Value;
        var identifier = Identifier.Create(command.Identifier).Value;
        var path = Path.Create(parentDepartment.Path.Value + "." + identifier.Value).Value;
        var depth = (short)(parentDepartment.Depth + Department.CHILD_DEPARTMENT_DEPTH);

        var departmentResult = Department.Create(
            departmentId,
            departmentName,
            identifier,
            path,
            depth,
            parentDepartment);

        if (departmentResult.IsFailure)
            return departmentResult.Error;

        //закрепление департамента за локациями
        var addedDepartmentResult = await AddDepartmentToLocation(
            departmentResult.Value,
            command.LocationIds,
            cancellationToken);
        
        if (addedDepartmentResult.IsFailure)
            return addedDepartmentResult.Error;
        
        //родительскому департаменту в список добавляем дочерний
        parentDepartment.AddChildDepartment(departmentResult.Value);

        //сохранение дочернего департамента в БД
        var result = await departmentsRepository.Add(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return departmentResult.Value.Id.Value;
    }

    private async Task<UnitResult<Error>> AddDepartmentToLocation(
        Department department,
        IEnumerable<Guid> locationIds,
        CancellationToken cancellationToken = default)
    {
        //получение локаций по Id
        var locationsResult = await GetLocations(locationIds, cancellationToken);
        if (locationsResult.IsFailure)
            return locationsResult.Error;
        
        //закрепление департамента за локациями
        foreach (var location in locationsResult.Value)
        {
            var departmentLocation = new DepartmentLocation(
                department.Id,
                location.Id,
                department,
                location
            );
            
            //департаменту в список добавляем локации
            department.AddLocation(departmentLocation);
            
            //локации в список добавляем департамент
            location.AddDepartment(departmentLocation);
        }

        return Result.Success<Error>();
    }

    private async Task<Result<IReadOnlyList<Location>, Error>> GetLocations(
        IEnumerable<Guid> locationIds, 
        CancellationToken cancellationToken = default)
    {
        var locationIdsList = locationIds.Select(LocationId.Create);

        var result = await locationsRepository.GetManyByIds(locationIdsList, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return result;
    }

    private async Task<UnitResult<Error>> CheckExistenceDepartmentByIdentifier(
        Identifier identifier,
        CancellationToken cancellationToken)
    {
        var departmentResult = await departmentsRepository.GetByIdentifier(identifier, cancellationToken);
        
        //если департамент с таким identifier уже существует, то вернется ошибка
        if (departmentResult.IsSuccess)
            return Errors.Department.AlreadyExist("Identifier");

        return Result.Success<Error>();
    }
}