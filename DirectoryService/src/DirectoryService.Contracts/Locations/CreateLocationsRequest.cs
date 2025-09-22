namespace DirectoryService.Contracts.Locations;

public record CreateLocationsRequest(
    string Name,
    string City,
    string Street,
    string House,
    string RoomNumber,
    string TimeZone);