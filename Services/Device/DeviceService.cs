using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.Device;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.DTOs.Playlist;

using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Services.Device;

public class DeviceService : IDeviceService
{
    private readonly AppDbContext _context;

    public DeviceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeviceSimpleResponseDto>> GetAllAsync()
    {
        return await _context.Devices
            .Include(d => d.Playlist)
            .Select(d => new DeviceSimpleResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                DeviceKey = d.DeviceKey,
                IpAddress = d.IpAddress,
                CreatedAt = d.CreatedAt,
                LastSeen = d.LastSeen,
                PlaylistId = d.PlaylistId,
                PlaylistName = d.Playlist!.Name
            })
            .ToListAsync();
    }

    public async Task<DeviceSimpleResponseDto?> GetByIdAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.Playlist)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null) return null;

        return new DeviceSimpleResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            DeviceKey = device.DeviceKey,
            IpAddress = device.IpAddress,
            CreatedAt = device.CreatedAt,
            LastSeen = device.LastSeen,
            PlaylistId = device.PlaylistId,
            PlaylistName = device.Playlist?.Name
        };
    }

    public async Task<DeviceSimpleResponseDto> CreateAsync(CreateDeviceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var exists = await _context.Devices
            .AnyAsync(d => d.DeviceKey == dto.DeviceKey);

        if (exists)
            throw new InvalidOperationException("Device already exists");

        var device = new Models.Device
        {
            Name = dto.Name,
            DeviceKey = dto.DeviceKey,
            IpAddress = dto.IpAddress,
            CreatedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return new DeviceSimpleResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            DeviceKey = device.DeviceKey,
            IpAddress = device.IpAddress,
            CreatedAt = device.CreatedAt,
            LastSeen = device.LastSeen
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<HeartbeatResponseDto> HandleHeartbeatAsync(
        string deviceKey,
        HeartbeatRequestDto request)
    {
        var device = await _context.Devices
            .Include(d => d.Playlist)
                .ThenInclude(p => p!.MediaFiles)
            .FirstOrDefaultAsync(d => d.DeviceKey == deviceKey);

        if (device == null)
        {
            device = new Models.Device
            {
                DeviceKey = deviceKey,
                Name = $"Device_{deviceKey}",
                IpAddress = request.IpAddress ?? "unknown",
                CreatedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return new HeartbeatResponseDto
            {
                ServerTime = DateTime.UtcNow,
                HasPlaylistAssigned = false,
                PlaylistChanged = false
            };
        }

        device.LastSeen = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(request.IpAddress))
            device.IpAddress = request.IpAddress;

        await _context.SaveChangesAsync();

        var response = new HeartbeatResponseDto
        {
            ServerTime = DateTime.UtcNow,
            HasPlaylistAssigned = device.PlaylistId.HasValue,
            PlaylistChanged = false
        };

        if (device.Playlist == null)
            return response;

        bool playlistChanged =
            request.CurrentPlaylistId != device.Playlist.Id;

        response.PlaylistChanged = playlistChanged;

        if (playlistChanged || request.CurrentPlaylistId == null)
        {
            response.Playlist = new PlaylistDetailResponseDto
            {
                Id = device.Playlist.Id,
                Name = device.Playlist.Name,
                CreatedAt = device.Playlist.CreatedAt,
                MediaFiles = device.Playlist.MediaFiles
                    .OrderBy(m => m.Id)
                    .Select(m => new MediaFileSimpleDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Url = m.Url,
                        UploadedAt = m.UploadedAt
                    })
                    .ToList()
            };
        }

        return response;
    }
}
