using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.MediaFile;
using DigitalSignageMVP.Services.MediaFiles;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaFilesController : ControllerBase
{
    private readonly IMediaFileService _mediaFileService;

    public MediaFilesController(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaFileResponseDto>>> Get(
        [FromQuery] string? name,
        [FromQuery] int? playlistId)
    {
        var result = await _mediaFileService.GetAsync(name, playlistId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MediaFileResponseDto>> GetById(int id)
    {
        var result = await _mediaFileService.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<MediaFileResponseDto>> Upload(
        [FromForm] UploadMediaFileDto request)
    {
        var result = await _mediaFileService.UploadAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediaFileService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}