using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.Device;

namespace DigitalSignageMVP.Services.Device;

public interface IDeviceService
{
    Task<IEnumerable<DeviceSimpleResponseDto>> GetAllAsync();
    Task<DeviceSimpleResponseDto?> GetByIdAsync(int id);
    Task<DeviceSimpleResponseDto> CreateAsync(CreateDeviceDto dto);
    Task<bool> DeleteAsync(int id);

    Task<HeartbeatResponseDto> HandleHeartbeatAsync(
        string deviceKey,
        HeartbeatRequestDto request);
}