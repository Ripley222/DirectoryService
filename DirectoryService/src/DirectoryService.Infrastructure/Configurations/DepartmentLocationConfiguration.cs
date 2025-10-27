using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_locations");
        
        builder
            .HasKey(dl => new {dl.DepartmentId, dl.LocationId})
            .HasName("pk_department_location");

        builder
            .Property(d => d.DepartmentId)
            .HasColumnName("department_id");
        
        builder
            .HasOne<Department>()
            .WithMany(d => d.Locations)
            .HasForeignKey(d => d.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(d => d.LocationId)
            .HasColumnName("location_id");
        
        builder
            .HasOne<Location>()
            .WithMany(l => l.Departments)
            .HasForeignKey(d => d.LocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}