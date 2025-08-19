using DirectoryService.Domain.Entities.LocationEntity;
using DirectoryService.Domain.Entities.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");
        
        builder
            .HasKey(dl => new {dl.DepartmentId, dl.LocationId})
            .HasName("pk_department_location");
        
        builder
            .HasOne(dl => dl.Department)
            .WithMany(d => d.Locations)
            .HasForeignKey(d => d.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(dl => dl.Location)
            .WithMany(l => l.Departments)
            .HasForeignKey(d => d.LocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}