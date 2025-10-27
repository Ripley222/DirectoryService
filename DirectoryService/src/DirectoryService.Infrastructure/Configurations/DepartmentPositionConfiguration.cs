using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.PositionEntity;
using DirectoryService.Domain.Entities.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("departments_positions");
        
        builder
            .HasKey(dl => new {dl.DepartmentId, dl.PositionId})
            .HasName("pk_department_position");
        
        builder
            .Property(d => d.DepartmentId)
            .HasColumnName("department_id");
        
        builder
            .HasOne<Department>()
            .WithMany(d => d.Positions)
            .HasForeignKey(d => d.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(d => d.PositionId)
            .HasColumnName("position_id");
        
        builder
            .HasOne<Position>()
            .WithMany(p => p.Departments)
            .HasForeignKey(d => d.PositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}