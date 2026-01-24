using System;

namespace DigitalSignageMVP.DTOs.Device;

public class CreateDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string DeviceKey { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}