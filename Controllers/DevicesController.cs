using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.Device;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.DTOs.Playlist;
using DigitalSignageMVP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public DevicesController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceSimpleResponseDto>>> GetDevices()
    {
        var devices = await _context.Devices
            .Include(p => p.Playlist)
            .ToListAsync();

        var response = devices.
            Select(d => new DeviceSimpleResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            DeviceKey = d.DeviceKey,
            IpAddress = d.IpAddress,
            CreatedAt = d.CreatedAt,
            LastSeen = d.LastSeen,
            PlaylistId = d.PlaylistId,
            PlaylistName = d.Playlist?.Name
        }).ToList();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceSimpleResponseDto>> GetDevice(int id)
    {
        var device = await _context.Devices
            .Where(d => d.Id == id)
            .Include(p => p.Playlist)
            .FirstOrDefaultAsync();

        if (device == null) return NotFound("Device not found");

        var response = new DeviceSimpleResponseDto
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
        
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        
        if (device == null) return NotFound("Device not found");
        
        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<DeviceSimpleResponseDto>> CreateDevice(
        [FromBody] CreateDeviceDto createDto
    )
    {
        var exists = await _context.Devices.
            AnyAsync(d => d.DeviceKey == createDto.DeviceKey);
        
        if (exists) return BadRequest("Device already exists");
        
        var device = new Device
        {
            Name = createDto.Name,
            DeviceKey = createDto.DeviceKey,
            IpAddress = createDto.IpAddress,
            CreatedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };
            
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        var response = new DeviceSimpleResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            DeviceKey = device.DeviceKey,
            IpAddress = device.IpAddress,
            CreatedAt = device.CreatedAt,
            LastSeen = device.LastSeen,
            PlaylistId = device.PlaylistId,
            PlaylistName = null
        };

        return CreatedAtAction(nameof(GetDevice), 
            new { id = device.Id }, response);
    }
    
    [HttpPost("{deviceKey}/heartbeat")]
    public async Task<ActionResult<HeartbeatResponseDto>> Heartbeat(
        string deviceKey,
        [FromBody] HeartbeatRequestDto request)
    {
        var device = await _context.Devices
            .Include(d => d.Playlist)
            .ThenInclude(p => p!.MediaFiles)
            .FirstOrDefaultAsync(d => d.DeviceKey == deviceKey);
        
        if (device == null)
        {
            device = new Device
            {
                DeviceKey = deviceKey,
                Name = $"Device_{deviceKey}",
                IpAddress = request.IpAddress ?? "unknown",
                LastSeen = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return Ok(new HeartbeatResponseDto
            {
                ServerTime = DateTime.UtcNow,
                HasPlaylistAssigned = false,
                PlaylistChanged = false,
                Playlist = null
            });
        }
        
        device.LastSeen = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(request.IpAddress))
        {
            device.IpAddress = request.IpAddress;
        }
        await _context.SaveChangesAsync();
        
        var response = new HeartbeatResponseDto
        {
            ServerTime = DateTime.UtcNow,
            HasPlaylistAssigned = device.PlaylistId.HasValue,
            PlaylistChanged = false,
            Playlist = null
        };
        
        if (device.Playlist != null)
        {
            bool playlistChanged = request.CurrentPlaylistId != device.Playlist.Id;
            response.PlaylistChanged = playlistChanged;

            if (playlistChanged || request.CurrentPlaylistId == null)
            {
                var orderedFiles = device.Playlist.MediaFiles?
                    .OrderBy(mf => mf.Id)
                    .ToList() ?? new List<MediaFile>();

                response.Playlist = new PlaylistDetailResponseDto
                {
                    Id = device.Playlist.Id,
                    Name = device.Playlist.Name,
                    CreatedAt = device.Playlist.CreatedAt,
                    MediaFiles = orderedFiles.Select(mf => new MediaFileSimpleDto
                    {
                        Id = mf.Id,
                        Name = mf.Name,
                        Url = mf.FilePath,
                        UploadedAt = mf.UploadedAt
                    }).ToList()
                };
            }
        }

        return Ok(response);
    }
}