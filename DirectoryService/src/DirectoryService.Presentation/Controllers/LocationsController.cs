using DirectoryService.Application.LocationsFeatures.Create;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Requests.Locations;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] AddLocationsRequest request,
        [FromServices] CreateLocationsHandler handler)
    {
        var command = new CreateLocationsCommand(
            request.Name,
            request.City,
            request.Street,
            request.House,
            request.RoomNumber,
            request.TimeZone);
        
        var result = await handler.Handle(command);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}