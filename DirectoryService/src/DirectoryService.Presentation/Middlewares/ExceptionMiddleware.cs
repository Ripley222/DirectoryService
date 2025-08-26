using DirectoryService.Domain.Shared;
using DirectoryService.Presentation.Response;

namespace DirectoryService.Presentation.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            //вызов следующего компонента middleware
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            var responseError = Error.Failure("server.internal", ex.Message);
            var envelope = Envelope.Error(responseError);
            
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            await httpContext.Response.WriteAsJsonAsync(envelope);
        }
    }
}