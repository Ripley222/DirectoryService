using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Positions.Commands;
using DirectoryService.Domain.Entities.PositionEntity.ValueObjects;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Shared.SharedKernel.Errors;

namespace DirectoryService.Application.PositionsFeatures.Create;

public class CreatePositionsCommandValidation : AbstractValidator<CreatePositionsCommand>
{
    public CreatePositionsCommandValidation()
    {
        RuleFor(c => c.Request.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Request.Description)
            .MaximumLength(LengthConstants.Length1000)
            .WithError(Errors.General.ValueIsInvalid("Description"));

        RuleFor(c => c.Request.DepartmentIds)
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