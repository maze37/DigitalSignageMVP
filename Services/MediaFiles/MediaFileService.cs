using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.Models;
using DigitalSignageMVP.Services.FileStorageService;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Services.MediaFiles;

public class MediaFileService : IMediaFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _storage;

    public MediaFileService(
        AppDbContext context,
        IFileStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<IEnumerable<MediaFileResponseDto>> GetAsync(
        string? name,
        int? playlistId)
    {
        var query = _context.MediaFiles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(f => f.Name.Contains(name));

        if (playlistId.HasValue)
            query = query.Where(f => f.PlaylistId == playlistId.Value);

        return await query
            .Select(f => new MediaFileResponseDto
            {
                Id = f.Id,
                Name = f.Name,
                Url = f.Url,
                UploadedAt = f.UploadedAt,
                PlaylistId = f.PlaylistId
            })
            .ToListAsync();
    }

    public async Task<MediaFileResponseDto?> GetByIdAsync(int id)
    {
        var file = await _context.MediaFiles.FindAsync(id);
        if (file == null) return null;

        return new MediaFileResponseDto
        {
            Id = file.Id,
            Name = file.Name,
            Url = file.Url,
            UploadedAt = file.UploadedAt,
            PlaylistId = file.PlaylistId
        };
    }

    public async Task<MediaFileResponseDto> UploadAsync(
        UploadMediaFileDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.File);

        if (request.File.Length == 0)
            throw new InvalidOperationException("File is empty");

        var url = await _storage.UploadAsync(request.File);

        var mediaFile = new MediaFile
        {
            Name = Path.GetFileNameWithoutExtension(request.File.FileName),
            Url = url,
            UploadedAt = DateTime.UtcNow
        };

        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();

        return new MediaFileResponseDto
        {
            Id = mediaFile.Id,
            Name = mediaFile.Name,
            Url = mediaFile.Url,
            UploadedAt = mediaFile.UploadedAt,
            PlaylistId = mediaFile.PlaylistId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mediaFile = await _context.MediaFiles.FindAsync(id);
        if (mediaFile == null) return false;

        await _storage.DeleteAsync(mediaFile.Url);

        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();

        return true;
    }
}
