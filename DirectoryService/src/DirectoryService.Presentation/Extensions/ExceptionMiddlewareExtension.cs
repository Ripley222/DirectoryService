using DirectoryService.Presentation.Middlewares;

namespace DirectoryService.Presentation.Extensions;

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}