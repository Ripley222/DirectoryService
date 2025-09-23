namespace DirectoryService.Contracts.Locations;

public record LocationDto(
    Guid LocationId,
    string LocationName,
    string LocationAddress,
    string LocationTimeZone,
    DateTime DateCreated);