using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalSignageMVP.Models;

public class MediaFile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    [StringLength(128)]
    public string FilePath { get; set; } = string.Empty;
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    public int? PlaylistId { get; set; }
    public Playlist? Playlist { get; set; }
}
