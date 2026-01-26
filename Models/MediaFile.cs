using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalSignageMVP.Models;

public class MediaFile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int? PlaylistId { get; set; }
    public Playlist? Playlist { get; set; }
}
