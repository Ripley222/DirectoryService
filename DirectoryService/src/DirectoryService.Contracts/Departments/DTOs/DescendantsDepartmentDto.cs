﻿namespace DirectoryService.Contracts.Departments.DTOs;

public sealed class DescendantsDepartmentDto()
{
    public Guid Id { get; init; }
    public Guid ParentId { get; init; }
    public string Name { get; init; } = null!;
    public string Identifier { get; init; } = null!;
    public string Path { get; init; } = null!;
    public short Depth { get; init; }
    public bool IsActive { get; init; }
    
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    
    public bool HasMoreChildren { get; init; }
}