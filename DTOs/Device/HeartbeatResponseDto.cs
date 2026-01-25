using System;
using DigitalSignageMVP.DTOs.Playlist;

namespace DigitalSignageMVP.DTOs.Device;

public class HeartbeatResponseDto
{
    public DateTime ServerTime { get; set; } = DateTime.UtcNow;
    public bool HasPlaylistAssigned { get; set; }
    public bool PlaylistChanged { get; set; }
    public PlaylistDetailResponseDto? Playlist { get; set; }
}