using DirectoryService.Application.Validation;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.PositionsFeatures.Create;

public class CreatePositionsCommandValidator : AbstractValidator<CreatePositionsCommand>
{
    public CreatePositionsCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MaximumLength(LengthConstants.Length1000)
            .WithError(Errors.General.ValueIsInvalid("Description"));

        RuleFor(c => c.DepartmentIds)
            .Must(i =>
            {
                //проверка на наличие дубликатов при помощи HashSet
                var set = new HashSet<Guid>();
                return i.All(set.Add);
            })
            .WithError(Errors.General.ValueIsInvalid("DepartmentIds"))
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid("DepartmentIds"));
    }
}