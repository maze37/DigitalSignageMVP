using System;

namespace DigitalSignageMVP.Models;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceKey { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public int? PlaylistId { get; set; }
    public Playlist? Playlist { get; set; }
}