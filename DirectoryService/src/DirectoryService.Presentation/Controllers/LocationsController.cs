using DirectoryService.Application.LocationsFeatures.Create;
using DirectoryService.Application.LocationsFeatures.Get;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateLocationsRequest request,
        [FromServices] CreateLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }

    [HttpGet]
    public async Task<ActionResult> Get(
        [FromQuery] GetLocationsRequest query,
        [FromServices] GetLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}