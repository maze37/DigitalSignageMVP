using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.DTOs.Playlist;
using DigitalSignageMVP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public PlaylistsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaylistResponseDto>>> GetPlaylists()
    {
        var playlists = await _context.Playlists
            .Include(p => p.MediaFiles)
            .ToListAsync();
        
        var response = playlists.Select(p => new PlaylistResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            CreatedAt = p.CreatedAt,
            MediaFilesCount = p.MediaFiles.Count
        }).ToList();
        
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PlaylistDetailResponseDto>> GetPlaylist(int id)
    {
        var playlist = await _context.Playlists
            .Include(p => p.MediaFiles)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if  (playlist == null) return NotFound();

        var response = new PlaylistDetailResponseDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            MediaFiles = playlist.MediaFiles.Select(f => new MediaFileSimpleDto
            {
                Id = f.Id,
                Name = f.Name,
                Url = $"{Request.Scheme}://{Request.Host}/{f.FilePath}",
                UploadedAt = f.UploadedAt
            }).ToList()
        };
        
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlaylist(int id)
    {
        var playlist = await _context.Playlists.FindAsync(id);
        if (playlist == null) return NotFound();
        
        var filesInPlaylist = await _context.MediaFiles
            .Where(p => p.PlaylistId == id)
            .ToListAsync();

        foreach (var file in filesInPlaylist)
        {
            file.PlaylistId = null;
        }
        
        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPost]
    public async Task<ActionResult<PlaylistResponseDto>> CreatePlaylist(
        [FromBody] CreatePlaylistDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        bool nameExists = await _context.Playlists
            .AnyAsync(p => p.Name.ToLower() == createDto.Name.ToLower());
        
        if (nameExists)
            return Conflict("Playlist name already exists");

        var playlist = new Playlist
        {
            Name = createDto.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };
            
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var response = new PlaylistResponseDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            MediaFilesCount = 0
        };
        
        return CreatedAtAction(nameof(GetPlaylist), 
            new { id = playlist.Id }, response);
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<PlaylistResponseDto>> UpdatePlaylist(
        int id,
        [FromBody] UpdatePlaylistDto updateDto)
    {
        var existingPlaylist = await _context.Playlists.FindAsync(id);
        if (existingPlaylist == null) return NotFound();
        
        if (!string.IsNullOrWhiteSpace(updateDto.Name))
        {
            if (updateDto.Name.ToLower() != existingPlaylist.Name.ToLower())
            {
                bool nameExist = await _context.Playlists
                    .AnyAsync(p => p.Name.ToLower() == updateDto.Name.ToLower());
                
                if (nameExist)
                    return Conflict($"Playlist {updateDto.Name} already exists");

                existingPlaylist.Name = updateDto.Name.Trim();
            }
        }
        
        await _context.SaveChangesAsync();

        var response = new PlaylistResponseDto
        {
            Id = existingPlaylist.Id,
            Name = existingPlaylist.Name,
            CreatedAt = existingPlaylist.CreatedAt,
            MediaFilesCount = await _context.MediaFiles
                .CountAsync(m => m.PlaylistId == id)
        };

        return Ok(response);
    }

    [HttpPatch("{playlistId}/media/{mediaFileId}/add")]
    public async Task<ActionResult> AddMediaToPlaylist(int playlistId, int mediaFileId)
    {
        var playlist =  await _context.Playlists.FindAsync(playlistId);
        if (playlist == null) return NotFound();
        
        var mediaFile = await _context.MediaFiles.FindAsync(mediaFileId);
        if (mediaFile == null) return NotFound();

        if (mediaFile.PlaylistId.HasValue)
        {
            if (mediaFile.PlaylistId.Value == playlistId)
                return BadRequest($"Media file {mediaFileId} is already in this playlist");
            else
                return BadRequest($"Media file {mediaFileId} is already in playlist {mediaFile.PlaylistId}");
        }
        
        mediaFile.PlaylistId = playlist.Id;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = $"Media {mediaFile.Name} file has been added to playlist {playlist.Name}",
            mediafileId = mediaFile.Id,
            playlistId = playlist.Id
        });
    }

    [HttpPatch("{playlistId}/media/{mediaFileId}/remove")]
    public async Task<ActionResult> RemoveMediaFromPlaylist(int playlistId, int mediaFileId)
    {
        var mediaFile = await _context.MediaFiles.FindAsync(mediaFileId);
        if (mediaFile == null) return NotFound();

        if (!mediaFile.PlaylistId.HasValue)
        {
            return BadRequest($"Media file {mediaFileId} is not in any playlist");
        }
        
        if (mediaFile.PlaylistId != playlistId)
        {
            return BadRequest($"Media file {mediaFileId} is in playlist {mediaFile.PlaylistId}");
        }

        mediaFile.PlaylistId = null;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = $"Media file {mediaFileId} has been removed from playlist {playlistId}",
            mediafileId = mediaFile.Id
        });
    }
}
