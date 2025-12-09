using arabia.DTOs.Requests;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace arabia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IServiceService serviceService, ILogger<ServicesController> logger)
    {
        _serviceService = serviceService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.ServiceResponse>>> GetAll()
    {
        var services = await _serviceService.GetAllAsync();

        return Ok(services);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.ServiceResponse>>> GetActive()
    {
        var services = await _serviceService.GetActiveAsync();

        return Ok(services);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.ServiceResponse>> GetById(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    [HttpPost]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<DTOs.Responses.ServiceResponse>> Create(
        CreateServiceRequest request
    )
    {
        var service = await _serviceService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<DTOs.Responses.ServiceResponse>> Update(
        int id,
        UpdateServiceRequest request
    )
    {
        var service = await _serviceService.UpdateAsync(id, request);

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _serviceService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
