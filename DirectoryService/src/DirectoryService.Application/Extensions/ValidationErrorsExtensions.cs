using DirectoryService.Domain.Shared;
using FluentValidation.Results;

namespace DirectoryService.Application.Extensions;

public static class ValidationErrorsExtensions
{
    public static Errors GetErrors(this ValidationResult validationResult)
    {
        return new Errors(validationResult.Errors.Select(e => 
            Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName)));
    }
}