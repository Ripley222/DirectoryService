using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Domain.Shared;
using FluentValidation;

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