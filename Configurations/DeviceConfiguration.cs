using DigitalSignageMVP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalSignageMVP.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.DeviceKey)
            .IsRequired();
        
        builder.HasIndex(d => d.DeviceKey)
            .IsUnique();
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(64);
        
        builder.Property(d => d.DeviceKey)
            .IsRequired()
            .HasMaxLength(64);
        
        builder.Property(d => d.IpAddress)
            .IsRequired()
            .HasMaxLength(128);
    }
}