using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Entities.LocationEntity.ValueObjects;

public record Address
{
    public string City { get; }
    public string Street { get; }
    public string House { get; }
    public string RoomNumber { get; }
    
    private Address(string city, string street, string house, string roomNumber)
    {
        City = city;
        Street = street;
        House = house;
        RoomNumber = roomNumber;
    }

    public static Result<Address, Error> Create(
        string city,
        string street, 
        string house, 
        string roomNumber)
    {
        if (string.IsNullOrWhiteSpace(city))
            return GeneralErrors.ValueIsRequired("City");
        
        if (string.IsNullOrWhiteSpace(street))
            return GeneralErrors.ValueIsRequired("Street");
        
        if (string.IsNullOrWhiteSpace(house))
            return GeneralErrors.ValueIsRequired("House");

        return new Address(city, street, house, roomNumber);
    }
}