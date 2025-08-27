using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Shared;

public class Error
{
    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType ErrorType { get; }
    public string? InvalidField { get; }

    private Error(string errorCode, string errorMessage, ErrorType errorType, string? invalidField = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        InvalidField = invalidField;
    }
    
    public static Error Validation(string errorCode, string errorMessage, string? invalidField = null) =>
        new Error(errorCode, errorMessage, ErrorType.Validation, invalidField);
    
    public static Error Failure(string errorCode, string errorMessage) =>
        new Error(errorCode, errorMessage, ErrorType.Failure);
    
    public static Error NotFound(string errorCode, string errorMessage) =>
        new Error(errorCode, errorMessage, ErrorType.NotFound);
    
    public static Error Conflict(string errorCode, string errorMessage) =>
        new Error(errorCode, errorMessage, ErrorType.Conflict);

    public ErrorList ToErrors() => new ErrorList([this]);
}