using System;
namespace DigitalSignageMVP.Models;

public class Device
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }

    public int? PlaylistId { get; set; } // Fk
    public Playlist? Playlist { get; set; }
}