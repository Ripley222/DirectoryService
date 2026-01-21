using CSharpFunctionalExtensions;
using DirectoryService.Application.DistributedCaching;
using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions.Caching;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;
using Errors = DirectoryService.Domain.Shared.Errors;
using Path = DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects.Path;

namespace DirectoryService.Application.DepartmentsFeatures.Create;

public class CreateDepartmentsHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ICacheService cacheService,
    IValidator<CreateDepartmentsCommand> validator,
    ILogger<CreateDepartmentsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateDepartmentsCommand command,
        CancellationToken cancellationToken)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var departmentId = DepartmentId.New();
        var departmentName = DepartmentName.Create(command.Request.Name).Value;
        var identifier = Identifier.Create(command.Request.Identifier).Value;

        //бизнес валидация
        //проверка на существование локаций
        var locationsExistResult = await locationsRepository.CheckActiveLocationsByIds(
            command.Request.LocationIds.Select(LocationId.Create), cancellationToken);

        if (locationsExistResult.IsFailure)
            return locationsExistResult.Error.ToErrors();

        //проверка на уникальность identifier
        var departmentExistWithIdentifier = await departmentsRepository.CheckByIdentifier(
            identifier, cancellationToken);

        if (departmentExistWithIdentifier.IsSuccess)
            return Errors.Department.AlreadyExist("Identifier").ToErrors();

        //проверка на наличие родительского департамента
        Department? parentDepartment = null;
        if (command.Request.ParentId is not null)
        {
            var parentDepartmentResult = await departmentsRepository.GetById(
                DepartmentId.Create(command.Request.ParentId.Value), cancellationToken);

            if (parentDepartmentResult.IsFailure)
                return Errors.Department.NotFound().ToErrors();

            parentDepartment = parentDepartmentResult.Value;
        }

        var path = parentDepartment is not null
            ? Path.Create(parentDepartment.Path.Value + "." + identifier.Value).Value
            : Path.Create(command.Request.Identifier).Value;

        var depth = parentDepartment is not null
            ? (short)(parentDepartment.Depth + Department.CHILD_DEPARTMENT_DEPTH)
            : (short)Department.MAIN_DEPARTMENT_DEPTH;

        //создание нового департамента
        var departmentResult = Department.Create(
            departmentId,
            departmentName,
            identifier,
            path,
            depth,
            parentDepartment);

        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        //привязка департамента к локациям
        List<DepartmentLocation> departmentLocations = [];
        foreach (var locationId in command.Request.LocationIds)
        {
            var departmentsLocations = new DepartmentLocation(
                departmentId,
                LocationId.Create(locationId));

            departmentLocations.Add(departmentsLocations);
        }

        departmentResult.Value.SetLocations(departmentLocations);

        //добавление нового департамента
        var result = await departmentsRepository.Add(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();
        
        //инвалидация кэша
        var key = CacheConstants.CACHING_DEPARTMENTS_KEY;
        await cacheService.RemoveByPrefixAsync(key, cancellationToken);

        logger.LogInformation("Created new Department with id {id}", departmentId.Value);

        return departmentId.Value;
    }
}