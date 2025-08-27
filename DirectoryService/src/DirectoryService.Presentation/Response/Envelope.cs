using DirectoryService.Domain.Shared;

namespace DirectoryService.Presentation.Response;

public class Envelope
{
    public object? Result { get; }
    public ErrorList? Errors { get; }
    public DateTime TimeGenerated { get; }

    private Envelope(object? result, ErrorList? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result = null) =>
        new(result, null);
    
    public static Envelope Error(ErrorList errorList) =>
        new(null, errorList);
}