using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.DTOs.Playlist;
using DigitalSignageMVP.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Services.Playlists;

public class PlaylistService : IPlaylistService
{
    private readonly AppDbContext _context;

    public PlaylistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlaylistResponseDto>> GetAllAsync()
    {
        return await _context.Playlists
            .Select(p => new PlaylistResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                CreatedAt = p.CreatedAt,
                MediaFilesCount = p.MediaFiles.Count
            })
            .ToListAsync();
    }

    public async Task<PlaylistDetailResponseDto?> GetByIdAsync(int id)
    {
        var playlist = await _context.Playlists
            .Include(p => p.MediaFiles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (playlist == null) return null;

        return new PlaylistDetailResponseDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            MediaFiles = playlist.MediaFiles.Select(f => new MediaFileSimpleDto
            {
                Id = f.Id,
                Name = f.Name,
                Url = f.Url,
                UploadedAt = f.UploadedAt
            }).ToList()
        };
    }

    public async Task<PlaylistResponseDto> CreateAsync(CreatePlaylistDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var exists = await _context.Playlists
            .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

        if (exists)
            throw new InvalidOperationException("Playlist name already exists");

        var playlist = new Playlist
        {
            Name = dto.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        return new PlaylistResponseDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            MediaFilesCount = 0
        };
    }

    public async Task<PlaylistResponseDto?> UpdateAsync(int id, UpdatePlaylistDto dto)
    {
        var playlist = await _context.Playlists.FindAsync(id);
        if (playlist == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name) &&
            !dto.Name.Equals(playlist.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _context.Playlists
                .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new InvalidOperationException("Playlist name already exists");

            playlist.Name = dto.Name.Trim();
        }

        await _context.SaveChangesAsync();

        return new PlaylistResponseDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            MediaFilesCount = await _context.MediaFiles
                .CountAsync(m => m.PlaylistId == id)
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var playlist = await _context.Playlists.FindAsync(id);
        if (playlist == null) return false;

        var mediaFiles = await _context.MediaFiles
            .Where(m => m.PlaylistId == id)
            .ToListAsync();

        foreach (var file in mediaFiles)
            file.PlaylistId = null;

        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddMediaAsync(int playlistId, int mediaFileId)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);
        var media = await _context.MediaFiles.FindAsync(mediaFileId);

        if (playlist == null || media == null)
            return false;

        if (media.PlaylistId.HasValue)
            throw new InvalidOperationException("Media file already assigned to playlist");

        media.PlaylistId = playlistId;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveMediaAsync(int playlistId, int mediaFileId)
    {
        var media = await _context.MediaFiles.FindAsync(mediaFileId);
        if (media == null) return false;

        if (media.PlaylistId != playlistId)
            throw new InvalidOperationException("Media file is in another playlist");

        media.PlaylistId = null;
        await _context.SaveChangesAsync();

        return true;
    }
}
