using CSharpFunctionalExtensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Positions.Commands;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;
using Errors = DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.PositionsFeatures.Create;

public class CreatePositionsHandler(
    IPositionsRepository positionsRepository,
    IDepartmentsRepository departmentsRepository,
    IValidator<CreatePositionsCommand> validator,
    ILogger<CreatePositionsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        CreatePositionsCommand command,
        CancellationToken cancellationToken = default)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var positionId = PositionId.New();
        var positionName = PositionName.Create(command.Request.Name).Value;
        var description = command.Request.Description;
        
        //бизнес валдиация
        //проверка на существование активной позиации с таким же названием
        var positionExist = await positionsRepository
            .CheckActivePositionsByName(positionName, cancellationToken);

        if (positionExist.IsSuccess)
            return Errors.Position.AlreadyExist("Name").ToErrors();
        
        //проверка на существование активных департаментов
        var departmentsExist = await departmentsRepository
            .CheckActiveDepartmentsByIds(command.Request.DepartmentIds.Select(DepartmentId.Create), cancellationToken);
        
        if (departmentsExist.IsFailure)
            return departmentsExist.Error.ToErrors();

        var positionResult = Position.Create(
            positionId,
            positionName,
            description);
        
        if (positionResult.IsFailure)
            return positionResult.Error.ToErrors();

        //привязка позиции к департаментам
        foreach (var departmentId in command.Request.DepartmentIds)
        {
            var departmentPositon = new DepartmentPosition(
                DepartmentId.Create(departmentId),
                positionId);
            
            positionResult.Value.AddDepartment(departmentPositon);
        }
        
        var result = await positionsRepository.Add(positionResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        logger.LogInformation("Created new Position with id {id}", positionId.Value);
        
        return positionId.Value;
    }
}