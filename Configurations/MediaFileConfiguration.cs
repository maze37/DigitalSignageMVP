using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigitalSignageMVP.Models;

namespace DigitalSignageMVP.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(x => x.Playlist)
            .WithMany(p => p.MediaFiles)
            .HasForeignKey(x => x.PlaylistId);
        
        builder.HasIndex(m => m.Name)
            .IsUnique();
    }
}
