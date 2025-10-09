namespace DirectoryService.Contracts.Locations.DTOs;

public record GetLocationsDto(
    IEnumerable<LocationDto> Locations);