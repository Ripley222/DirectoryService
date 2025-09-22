using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Application.DepartmentsFeatures.UpdateParent;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDepartmentsRequest request,
        [FromServices] CreateDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }

    [HttpPut("{departmentId}/locations")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid departmentId,
        [FromBody] IEnumerable<Guid> locationIds,
        [FromServices] UpdateDepartmentLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new UpdateDepartmentLocationsRequest(departmentId, locationIds), 
            cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }

    [HttpPut("{departmentId}/parent")]
    public async Task<IActionResult> UpdateParent(
        [FromRoute] Guid departmentId,
        [FromBody] Guid? parentId,
        [FromServices] UpdateDepartmentParentHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateDepartmentParentRequest(
            departmentId,
            parentId);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}