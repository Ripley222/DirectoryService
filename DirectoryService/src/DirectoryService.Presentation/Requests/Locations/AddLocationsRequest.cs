namespace DirectoryService.Presentation.Requests.Locations;

public record AddLocationsRequest(
    string Name,
    string City,
    string Street,
    string House,
    string RoomNumber,
    string TimeZone);