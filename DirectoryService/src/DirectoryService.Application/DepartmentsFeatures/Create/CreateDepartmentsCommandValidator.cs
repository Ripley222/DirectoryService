using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.Create;

public class CreateDepartmentsCommandValidator : AbstractValidator<CreateDepartmentsRequest>
{
    public CreateDepartmentsCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(c => c.Identifier)
            .MustBeValueObject(Identifier.Create);

        RuleFor(c => c.ParentId)
            .Must(p => p == null || p.Value != Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid("ParentId"));

        RuleFor(c => c.LocationIds)
            .Must(i =>
            {
                //проверка на наличие дубликатов при помощи HashSet
                var set = new HashSet<Guid>();
                return i.All(set.Add);
            })
            .WithError(Errors.General.ValueIsInvalid("LocationIds"))
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid("LocationIds"));
    }
}