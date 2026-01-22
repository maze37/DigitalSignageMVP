using System;

namespace DigitalSignageMVP.DTOs.Playlist;

public class PlaylistResponseDto
{
    public int Id  { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MediaFilesCount { get; set; }
}