using DirectoryService.Domain.Entities.DepartmentEntity;
using DirectoryService.Domain.Entities.Ids;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        
        builder.HasKey(d => d.Id)
            .HasName("pk_department");

        builder.Property(d => d.Id)
            .HasConversion(
                d => d.Value,
                id => DepartmentId.Create(id))
            .HasColumnName("id");

        builder.ComplexProperty(d => d.DepartmentName, db =>
        {
            db.Property(n => n.Value)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(LengthConstants.Length150);
        });

        builder.ComplexProperty(d => d.Identifier, ib =>
        {
            ib.Property(i => i.Value)
                .IsRequired()
                .HasColumnName("identifier")
                .HasMaxLength(LengthConstants.Length150);
        });
        
        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .IsRequired(false);

        builder.ComplexProperty(d => d.Path, pb =>
        {
            pb.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("path");
        });
        
        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");
        
        builder
            .HasOne(d => d.Parent)
            .WithMany(d => d.ChildDepartments)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
        
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