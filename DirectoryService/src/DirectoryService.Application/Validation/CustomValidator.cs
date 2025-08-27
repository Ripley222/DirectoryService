using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Application.Validation;

public static class CustomValidator
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);
            
            if (result.IsSuccess)
                return;
            
            context.AddFailure(new ValidationFailure
            {
                ErrorCode = result.Error.ErrorCode,
                ErrorMessage = result.Error.ErrorMessage,
                PropertyName = result.Error.InvalidField
            });
        });
    }
}