using DirectoryService.Application.DepartmentsFeatures.Create;
using DirectoryService.Application.DepartmentsFeatures.GetDescendants;
using DirectoryService.Application.DepartmentsFeatures.GetRootsWithNChildren;
using DirectoryService.Application.DepartmentsFeatures.GetTopByPosition;
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
    [HttpGet("top-positions")]
    public async Task<IActionResult> GetTopDepartments(
        [FromServices] GetTopDepartmentsByPositionsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(cancellationToken);

        var envelope = Envelope.Ok(result);
        return Ok(envelope);
    }

    [HttpGet("roots")]
    public async Task<IActionResult> GetRoots(
        [FromQuery] GetNChildDepartmentsRequest request,
        [FromServices] GetRootsWithNChildrenDepartmentsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new GetNChildDepartmentsQuery(
                request.GetDepartmentsWithPagination.Page,
                request.GetDepartmentsWithPagination.Size,
                request.Prefetch), 
            cancellationToken);
        
        var envelope = Envelope.Ok(result);
        return Ok(envelope);
    }

    [HttpGet("{parentId:guid}/children")]
    public async Task<IActionResult> GetDescendants(
        [FromRoute] Guid parentId,
        [FromQuery] GetDepartmentsWithPaginationRequest request,
        [FromServices] GetDescendantsDepartmentsLazyWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new GetDescendantDepartmentsWithPaginationQuery(parentId, request.Page, request.Size),
            cancellationToken);
        
        var envelope = Envelope.Ok(result);
        return Ok(envelope);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
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
    public async Task<IActionResult> Update(
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
    public async Task<IActionResult> UpdateParent(
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
}