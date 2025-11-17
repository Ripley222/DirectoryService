using DirectoryService.Application.LocationsFeatures.Create;
using DirectoryService.Application.LocationsFeatures.Get;
using DirectoryService.Contracts.Locations.Commands;
using DirectoryService.Contracts.Locations.Queries;
using DirectoryService.Contracts.Locations.Requests;
using Microsoft.AspNetCore.Mvc;
using Shared.Framework.Extensions;
using Shared.SharedKernel;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(
        [FromQuery] GetLocationsRequest request,
        [FromServices] GetLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(new GetLocationsQuery(request), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateLocationsRequest request,
        [FromServices] CreateLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(new CreateLocationsCommand(request), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}