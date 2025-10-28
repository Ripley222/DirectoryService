using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        
        builder.HasKey(l => l.Id)
            .HasName("pk_location");

        builder.Property(l => l.Id)
            .HasConversion(
                l => l.Value,
                id => LocationId.Create(id))
            .HasColumnName("id");

        builder.ComplexProperty(l => l.LocationName, lb =>
        {
            lb.Property(ln => ln.Value)
                .IsRequired()
                .HasMaxLength(LengthConstants.Length120)
                .HasColumnName("name");
        });

        builder.ComplexProperty(l => l.Address, lb =>
        {
            lb.Property(a => a.City)
                .IsRequired()
                .HasColumnName("city");
            
            lb.Property(a => a.Street)
                .IsRequired()
                .HasColumnName("street");
            
            lb.Property(a => a.House)
                .IsRequired()
                .HasColumnName("house");
            
            lb.Property(a => a.RoomNumber)
                .IsRequired(false)
                .HasColumnName("room_number");
        });

        builder.ComplexProperty(l => l.TimeZone, lb =>
        {
            lb.Property(t => t.Value)
                .IsRequired()
                .HasColumnName("time_zone");
        });
        
        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(l => l.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");
        
        builder.Property(d => d.DeletedAt)
            .IsRequired()
            .HasColumnName("deleted_at");
        
        builder.Property<bool>("_isActive")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_active");
    }
}