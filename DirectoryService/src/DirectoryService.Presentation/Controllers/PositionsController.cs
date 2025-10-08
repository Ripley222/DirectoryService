using DirectoryService.Application.PositionsFeatures.Create;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.Positions.Commands;
using DirectoryService.Contracts.Positions.Requests;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePositionsRequest request,
        [FromServices] CreatePositionsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(new CreatePositionsCommand(request), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}