using Microsoft.EntityFrameworkCore;
using DigitalSignageMVP.Configurations;
using DigitalSignageMVP.Models;

namespace DigitalSignageMVP.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Device> Devices { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new MediaFileConfiguration());
        modelBuilder.ApplyConfiguration(new PlaylistConfiguration());
        modelBuilder.ApplyConfiguration(new PlaylistConfiguration());
        
        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.MediaFiles)
            .WithOne(m => m.Playlist)
            .HasForeignKey(m => m.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}