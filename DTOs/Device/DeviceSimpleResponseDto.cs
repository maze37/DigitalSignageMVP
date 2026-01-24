using System;

namespace DigitalSignageMVP.DTOs.Device;

public class DeviceSimpleResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceKey { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? PlaylistId { get; set; }
    public string? PlaylistName { get; set; }
}