using DirectoryService.Contracts.Departments.Commands;
using FluentValidation;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Application.DepartmentsFeatures.SoftDelete;

public class SoftDeleteDepartmentsCommandValidation : AbstractValidator<DeleteDepartmentsCommand>
{
    public SoftDeleteDepartmentsCommandValidation()
    {
        RuleFor(d => d.DepartmentId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("DepartmentId"));
    }
}