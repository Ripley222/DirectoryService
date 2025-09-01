using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.DepartmentsFeatures.UpdateLocations;

public class UpdateDepartmentLocationsCommandValidation : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidation()
    {
        RuleFor(u => u.DepartmentId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid("DepartmentId"));
        
        RuleFor(u => u.LocationIds)
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid("LocationIds"))
            .Must(i =>
            {
                //проверка на наличие дубликатов при помощи HashSet
                var set = new HashSet<Guid>();
                return i.All(set.Add);
            })
            .WithError(Errors.General.ValueIsInvalid("LocationIds"));
    }
}