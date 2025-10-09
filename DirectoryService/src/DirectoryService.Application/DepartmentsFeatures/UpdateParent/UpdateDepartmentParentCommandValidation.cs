using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.UpdateParent;

public class UpdateDepartmentParentCommandValidation : AbstractValidator<UpdateDepartmentParentCommand>
{
    public UpdateDepartmentParentCommandValidation()
    {
        RuleFor(u => u.DepartmentId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("DepartmentId"));

        RuleFor(u => u.ParentId)
            .Must(i => i != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("ParentId"));
        
        RuleFor(u => new { u.DepartmentId, u.ParentId })
            .Must(items => items.DepartmentId != items.ParentId)
            .WithError(Errors.General.ValueIsInvalid("DepartmentId"));
    }
}