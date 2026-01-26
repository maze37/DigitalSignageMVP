using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.Playlist;
using DigitalSignageMVP.Services.Playlists;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _service;

    public PlaylistsController(IPlaylistService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var playlist = await _service.GetByIdAsync(id);
        return playlist == null ? NotFound() : Ok(playlist);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlaylistDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePlaylistDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{playlistId}/media/{mediaFileId}/add")]
    public async Task<IActionResult> AddMedia(int playlistId, int mediaFileId)
    {
        await _service.AddMediaAsync(playlistId, mediaFileId);
        return Ok();
    }

    [HttpPatch("{playlistId}/media/{mediaFileId}/remove")]
    public async Task<IActionResult> RemoveMedia(int playlistId, int mediaFileId)
    {
        await _service.RemoveMediaAsync(playlistId, mediaFileId);
        return Ok();
    }
}