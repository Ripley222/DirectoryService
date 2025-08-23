using DirectoryService.Domain.Shared;

namespace DirectoryService.Presentation.Response;

public class Envelope
{
    public object? Result { get; }
    public Errors? Errors { get; }
    public DateTime TimeGenerated { get; }

    private Envelope(object? result, Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result = null) =>
        new(result, null);
    
    public static Envelope Error(Errors errors) =>
        new(null, errors);
}