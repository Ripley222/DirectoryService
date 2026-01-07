namespace DirectoryService.Contracts.Locations.DTOs;

public record LocationDto(
    Guid LocationId,
    string LocationName,
    string LocationAddress,
    string LocationTimeZone,
    DateTime DateCreated,
    bool IsActive);