using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");
        
        builder.HasKey(p => p.Id)
            .HasName("pk_position");

        builder.Property(p => p.Id)
            .HasConversion(
                p => p.Value,
                id => PositionId.Create(id))
            .HasColumnName("id");

        builder.ComplexProperty(p => p.PositionName, pb =>
        {
            pb.Property(n => n.Value)
                .IsRequired()
                .HasMaxLength(LengthConstants.Length100)
                .HasColumnName("position_name");
        });
        
        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(LengthConstants.Length1000)
            .HasColumnName("description");
        
        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(d => d.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");
        
        builder.Property<bool>("_isActive")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_active");
    }
}