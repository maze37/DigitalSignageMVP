using System;

namespace DigitalSignageMVP.DTOs.MediaFile;

public class MediaFileSimpleDto 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}