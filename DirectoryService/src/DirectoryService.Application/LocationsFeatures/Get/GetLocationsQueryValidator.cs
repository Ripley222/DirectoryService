using DirectoryService.Contracts.Locations.Queries;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Shared.Core.Validation;
using Errors = Shared.SharedKernel.Errors.Errors;

namespace DirectoryService.Application.LocationsFeatures.Get;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(g => g.Request.Page)
            .Must(p => p > 0)
            .WithError(Errors.General.ValueIsInvalid("Page"));

        RuleFor(g => g.Request.PageSize)
            .Must(p => p is > 0 and < LengthConstants.Length100)
            .WithError(Errors.General.ValueIsInvalid("PageSize"));
    }
}