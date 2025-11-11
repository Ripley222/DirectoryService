using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.GetDescendants;
using DirectoryService.Application.DepartmentsFeatures.GetRootsWithNChildren;
using DirectoryService.Application.DepartmentsFeatures.GetTopByPosition;
using DirectoryService.Application.DepartmentsFeatures.SoftDelete;
using DirectoryService.Application.DepartmentsFeatures.UpdateLocations;
using DirectoryService.Application.DepartmentsFeatures.UpdateParent;
using DirectoryService.Contracts.Departments.Commands;
using DirectoryService.Contracts.Departments.Queries;
using DirectoryService.Contracts.Departments.Requests;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpGet("departments/top")]
    public async Task<IActionResult> TopDepartments(
        [FromServices] GetTopDepartmentsByPositionsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        return Ok(envelope);
    }

    [HttpGet("roots")]
    public async Task<IActionResult> Roots(
        [FromQuery] GetNChildDepartmentsRequest request,
        [FromServices] GetRootsWithNChildrenDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new GetNChildDepartmentsQuery(
                request.Page,
                request.Size,
                request.Prefetch), 
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();
        
        var envelope = Envelope.Ok(result.Value);
        return Ok(envelope);
    }

    [HttpGet("{parentId:guid}/children")]
    public async Task<IActionResult> Descendants(
        [FromRoute] Guid parentId,
        [FromQuery] GetDepartmentsWithPaginationRequest request,
        [FromServices] GetDescendantsDepartmentsLazyWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new GetDescendantDepartmentsWithPaginationQuery(parentId, request.Page, request.Size),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();
        
        var envelope = Envelope.Ok(result.Value);
        return Ok(envelope);
    }

    [HttpPost]
    public async Task<IActionResult> Department(
        [FromBody] CreateDepartmentsRequest request,
        [FromServices] CreateDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(new CreateDepartmentsCommand(request), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);

        return Ok(envelope);
    }

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<IActionResult> Department(
        [FromRoute] Guid departmentId,
        [FromBody] IEnumerable<Guid> locationIds,
        [FromServices] UpdateDepartmentLocationsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new UpdateDepartmentLocationsCommand(departmentId, locationIds),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);

        return Ok(envelope);
    }

    [HttpPut("{departmentId:guid}/parent")]
    public async Task<IActionResult> Parent(
        [FromRoute] Guid departmentId,
        [FromBody] Guid? parentId,
        [FromServices] UpdateDepartmentParentHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new UpdateDepartmentParentCommand(departmentId, parentId), 
            cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);

        return Ok(envelope);
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] Guid departmentId,
        [FromServices] SoftDeleteDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new DeleteDepartmentsCommand(departmentId),
            cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        var envelope = Envelope.Ok(result.Value);
        
        return Ok(envelope);
    }
}