using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Domain.Entities.DepartmentEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.Create;

public class CreateDepartmentsCommandValidation : AbstractValidator<CreateDepartmentsCommand>
{
    public CreateDepartmentsCommandValidation()
    {
        RuleFor(c => c.Request.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(c => c.Request.Identifier)
            .MustBeValueObject(Identifier.Create);

        RuleFor(c => c.Request.ParentId)
            .Must(p => p == null || p.Value != Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid("ParentId"));

        RuleFor(c => c.Request.LocationIds)
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