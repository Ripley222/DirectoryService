using CSharpFunctionalExtensions;

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

    public static Result<Address> Create(
        string city,
        string street, 
        string house, 
        string roomNumber)
    {
        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>("City cannot be empty");
        
        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>("Street cannot be empty");
        
        if (string.IsNullOrWhiteSpace(house))
            return Result.Failure<Address>("House cannot be empty");

        return new Address(city, street, house, roomNumber);
    }
}