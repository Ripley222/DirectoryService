namespace DirectoryService.Application.LocationsFeatures.Create;

public record CreateLocationsCommand(
    string Name,
    string City,
    string Street,
    string House,
    string RoomNumber,
    string TimeZone);