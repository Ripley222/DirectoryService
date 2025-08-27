using DirectoryService.Domain.Shared;
using FluentValidation.Results;

namespace DirectoryService.Application.Extensions;

public static class ValidationErrorsExtensions
{
    public static ErrorList GetErrors(this ValidationResult validationResult)
    {
        var validationErrors = validationResult.Errors;

        var errors = validationErrors.Select(e =>
            Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName));
        
        return new ErrorList(errors);
    }
}