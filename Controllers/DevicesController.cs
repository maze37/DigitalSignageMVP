using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.Device;
using DigitalSignageMVP.Services.Device;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignageMVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _service;

    public DevicesController(IDeviceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await _service.GetByIdAsync(id);
        return device == null ? NotFound() : Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDeviceDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Id }, result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{deviceKey}/heartbeat")]
    public async Task<IActionResult> Heartbeat(
        string deviceKey,
        HeartbeatRequestDto request)
    {
        var response = await _service
            .HandleHeartbeatAsync(deviceKey, request);

        return Ok(response);
    }
}