using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Entities.Relationships;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

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
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var positionId = PositionId.New();
        var positionName = PositionName.Create(command.Name).Value;
        var description = command.Description;
        
        var positionExist = await positionsRepository
            .GetPositionsByName(positionName, cancellationToken);

        if (positionExist.IsSuccess)
        {
            if (positionExist.Value.Any(position => position.IsActive()))
            {
                return Errors.Position.AlreadyExist("Name").ToErrors();
            }
        }
        
        var departmentsExist = await departmentsRepository
            .GetManyById(command.DepartmentIds.Select(DepartmentId.Create), cancellationToken);
        
        if (departmentsExist.IsFailure)
            return departmentsExist.Error.ToErrors();

        if (departmentsExist.Value.Any(department => !department.IsActive()))
            return Errors.Department.NotActive().ToErrors();

        var positionResult = Position.Create(
            positionId,
            positionName,
            description);
        
        if (positionResult.IsFailure)
            return positionResult.Error.ToErrors();

        foreach (var department in departmentsExist.Value)
        {
            var departmentPositon = new DepartmentPosition(
                department.Id,
                positionId,
                department,
                positionResult.Value);
            
            department.AddPosition(departmentPositon);
            
            positionResult.Value.AddDepartment(departmentPositon);
        }
        
        var result = await positionsRepository.Add(positionResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        logger.LogInformation("Created Position with id {id}", positionId.Value);
        
        return positionId.Value;
    }
}