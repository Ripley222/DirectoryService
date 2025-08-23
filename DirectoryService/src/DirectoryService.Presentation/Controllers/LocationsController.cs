using DirectoryService.Application.LocationFeatures.Add;
using DirectoryService.Presentation.Requests.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add(
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
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }
}