namespace DirectoryService.Application.LocationFeatures.Create;

public record CreateLocationsCommand(
    string Name,
    string City,
    string Street,
    string House,
    string RoomNumber,
    string TimeZone);