using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Requests.Departments;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AddDepartmentsRequest request,
        [FromServices] CreateDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request.ToCommand(), cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }

    [HttpPut("{departmentId}/locations")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsRequest request,
        [FromServices] UpdateDepartmentLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request.ToCommand(departmentId), cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}