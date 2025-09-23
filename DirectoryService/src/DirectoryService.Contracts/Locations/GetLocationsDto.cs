namespace DirectoryService.Contracts.Locations;

public record GetLocationsDto(
    IEnumerable<LocationDto> Locations);