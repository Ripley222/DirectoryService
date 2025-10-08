using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Queries;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.GetDescendants;

public class GetDescendantDepartmentsWithPaginationQueryValidation : AbstractValidator<GetDescendantDepartmentsWithPaginationQuery>
{
    public GetDescendantDepartmentsWithPaginationQueryValidation()
    {
        RuleFor(g => g.DepartmentId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("DepartmentId"));
    }
}