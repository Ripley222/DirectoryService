namespace DirectoryService.Application.LocationFeatures.Add;

public record CreateLocationsCommand(
    string Name,
    string City,
    string Street,
    string House,
    string RoomNumber,
    string TimeZone);