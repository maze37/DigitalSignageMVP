using DigitalSignageMVP.Data;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.Models;
using DigitalSignageMVP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaFilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MediaFilesController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaFileResponseDto>>> GetMediaFiles(
        [FromQuery] string? name = null,
        [FromQuery] int? playlistId = null)
    {
        var query = _context.MediaFiles.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(f => f.Name.Contains(name));
        
        if (playlistId.HasValue)
            query = query.Where(f => f.PlaylistId == playlistId.Value);
        
        var files = await query.ToListAsync();
        
        var response = files.Select(f => new MediaFileResponseDto()
        {
            Id  = f.Id,
            Name = f.Name,
            Url = $"{Request.Scheme}://{Request.Host}/{f.FilePath}",
            UploadedAt = f.UploadedAt,
            PlaylistId = f.PlaylistId
        }).ToList();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaFileResponseDto>> GetMediaFile(int id)
    {
        var file = await _context.MediaFiles.FindAsync(id);
        if (file == null) return NotFound();
        
        return Ok(new MediaFileResponseDto()
        {
            Id = file.Id,
            Name = file.Name,
            Url = $"{Request.Scheme}://{Request.Host}/{file.FilePath}",
            UploadedAt = file.UploadedAt,
            PlaylistId = file.PlaylistId
        });
    }
    
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<MediaFileResponseDto>> UploadMediaFile([FromForm] UploadMediaFileDto request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("File is empty");
        
        var extension = Path.GetExtension(request.File.FileName).ToLower();
        
        var allowedExtensions = FileTypeValidator.GetAllowedExtensions();
        if (!FileTypeValidator.IsExtensionAllowed(extension))
        {
            return BadRequest("Invalid file extension. " +
                              "Allowed extensions are: " + string.Join(", ", allowedExtensions));
        }
        
        var name =  Path.GetFileNameWithoutExtension(request.File.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        
        var folder =  Path.Combine("wwwroot", "uploads");
        Directory.CreateDirectory(folder);
        var filePath = Path.Combine(folder, fileName);
        
        using var stream = new FileStream(filePath, FileMode.Create);
        await request.File.CopyToAsync(stream);

        var mediaFile = new MediaFile
        {
            Name = name,
            FilePath = $"uploads/{fileName}",
            UploadedAt = DateTime.UtcNow,
        };
        
        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();

        var response = new MediaFileResponseDto
        {
            Id = mediaFile.Id,
            Name = mediaFile.Name,
            Url = $"{Request.Scheme}://{Request.Host}/{mediaFile.FilePath}",
            UploadedAt = mediaFile.UploadedAt,
            PlaylistId = mediaFile.PlaylistId
        };
        
        return CreatedAtAction(nameof(GetMediaFile), new { id = mediaFile.Id }, response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMediaFile(int id)
    {
        var mediaFile = await _context.MediaFiles.FindAsync(id);
        if (mediaFile == null) return NotFound();
        
        var fullPath = Path.Combine("wwwroot", "uploads", mediaFile.FilePath);
        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
        
        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}