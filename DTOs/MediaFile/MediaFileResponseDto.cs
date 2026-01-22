namespace DigitalSignageMVP.DTOs.MediaFile;

public class MediaFileResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public int? PlaylistId { get; set; }
    public string Url { get; set; } = string.Empty;
}