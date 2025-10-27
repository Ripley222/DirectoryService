using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.DepartmentsFeatures.SoftDelete;

public class SoftDeleteDepartmentsHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    IValidator<DeleteDepartmentsCommand> validator,
    ILogger<SoftDeleteDepartmentsHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteDepartmentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var departmentId = DepartmentId.Create(command.DepartmentId);

        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();

        using var transaction = transactionResult.Value;

        var departmentsResult = await departmentsRepository.GetByIdWithLock(departmentId, cancellationToken);
        if (departmentsResult.IsFailure)
        {
            transaction.Rollback();
            return departmentsResult.Error.ToErrors();
        }

        if (departmentsResult.Value.IsActive() is false)
        {
            transaction.Rollback();
            return Errors.Department.NotActive().ToErrors();
        }

        var oldPath = departmentsResult.Value.Path;

        var deactivateResult = departmentsResult.Value.Deactivate();
        if (deactivateResult.IsFailure)
        {
            transaction.Rollback();
            return deactivateResult.Error.ToErrors();
        }

        var updateDepartmentResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (updateDepartmentResult.IsFailure)
        {
            transaction.Rollback();
            return updateDepartmentResult.Error.ToErrors();
        }

        var lockDescendantsResult = await departmentsRepository.LockDescendants(departmentsResult.Value.Path);
        if (lockDescendantsResult.IsFailure)
        {
            transaction.Rollback();
            return lockDescendantsResult.Error.ToErrors();
        }

        var updateDescendantsResult = await departmentsRepository.UpdateDescendantDepartments(
            departmentsResult.Value, oldPath);

        if (updateDescendantsResult.IsFailure)
        {
            transaction.Rollback();
            return updateDescendantsResult.Error.ToErrors();
        }
        
        var updateRelationshipsResult = await departmentsRepository.UpdateRelationships(departmentId);
        if (updateRelationshipsResult.IsFailure)
        {
            transaction.Rollback();
            return updateRelationshipsResult.Error.ToErrors();
        }

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();
        
        logger.LogInformation("Soft deleted departments with id {departmentId}", departmentId.Value);
        
        return departmentId.Value;
    }
}