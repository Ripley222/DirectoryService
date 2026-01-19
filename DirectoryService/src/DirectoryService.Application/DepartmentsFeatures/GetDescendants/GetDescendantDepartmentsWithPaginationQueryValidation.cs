using DirectoryService.Contracts.Departments.Queries;
using FluentValidation;
using Shared.Core.Validation;
using Shared.SharedKernel.Errors;

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