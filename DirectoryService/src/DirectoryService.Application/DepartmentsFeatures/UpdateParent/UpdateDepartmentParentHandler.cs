using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Repositories;
using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.DepartmentsFeatures.UpdateParent;

public class UpdateDepartmentParentHandler(
    IDepartmentsRepository repository,
    ITransactionManager transactionManager,
    IValidator<UpdateDepartmentParentCommand> validator,
    ILogger<UpdateDepartmentParentHandler> logger)
{
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateDepartmentParentCommand command,
        CancellationToken cancellationToken)
    {
        //валидация входных параметров
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.GetErrors();

        var departmentId = DepartmentId.Create(command.DepartmentId);

        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();
        
        using var transaction = transactionResult.Value;
        
        //бизнес валидация
        //получение департаментов с применением пассивной блокировки и проверка на активность
        var departmentResult = await repository.GetByIdWithLock(departmentId, cancellationToken);
        if (departmentResult.IsFailure || departmentResult.Value.IsActive() is false)
        {
            transaction.Rollback();
            return departmentResult.Error.ToErrors();
        }

        Department? parentDepartment = null;
        if (command.ParentId is not null)
        {
            var parentDepartmentId = DepartmentId.Create((Guid)command.ParentId);
            
            //проверка на то, что родительский департмент не является дочерним 
            var isDescendantsResult = await repository.IsDescendants(
                departmentId, 
                parentDepartmentId);

            if (isDescendantsResult.IsFailure)
            {
                transaction.Rollback();
                return isDescendantsResult.Error.ToErrors();
            }
            
            var parentDepartmentResult = await repository.GetByIdWithLock(parentDepartmentId, cancellationToken); 
            if (parentDepartmentResult.IsFailure ||  parentDepartmentResult.Value.IsActive() is false)
            {
                transaction.Rollback();
                return parentDepartmentResult.Error.ToErrors();
            }
            
            parentDepartment = parentDepartmentResult.Value;
        }
        
        //сохранение старого пути перед изменением сущности
        var oldPath = departmentResult.Value.Path;
        
        //изменение департамента в доменной сущности
        var setParentResult = departmentResult.Value.SetParent(parentDepartment);
        if (setParentResult.IsFailure)
        {
            transaction.Rollback();
            return setParentResult.Error.ToErrors();
        }
        
        //применение изменений в БД
        var updateDepartmentResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (updateDepartmentResult.IsFailure)
            return updateDepartmentResult.Error.ToErrors();

        //применение пассивной блокировки к дочерним подразделениям
        var lockDescendantsResult = await repository.LockDescendants(departmentResult.Value.Path);
        if (lockDescendantsResult.IsFailure)
        {
            transaction.Rollback();
            return lockDescendantsResult.Error.ToErrors();
        }
        
        //изменение дочерних департментов
        var updateDescendantDepartments = await repository.UpdateDescendantDepartments(
            departmentResult.Value,
            oldPath);
        
        if (updateDescendantDepartments.IsFailure)
            return updateDescendantDepartments.Error.ToErrors();
        
        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();
        
        logger.LogInformation("Updated parent department for department with id {departmentId}", command.DepartmentId);

        return departmentResult.Value.Id.Value;
    }
}