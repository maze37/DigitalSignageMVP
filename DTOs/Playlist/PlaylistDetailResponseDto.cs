using System;
using System.Collections.Generic;
using DigitalSignageMVP.DTOs.MediaFile;

namespace DigitalSignageMVP.DTOs.Playlist;

public class PlaylistDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<MediaFileSimpleDto> MediaFiles { get; set; } = new();
}