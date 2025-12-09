using arabia.DTOs.Requests;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace arabia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessesController : ControllerBase
{
    private readonly IBusinessService _businessService;
    private readonly ILogger<BusinessesController> _logger;

    public BusinessesController(
        IBusinessService businessService,
        ILogger<BusinessesController> logger
    )
    {
        _businessService = businessService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.BusinessResponse>>> GetAll()
    {
        var businesses = await _businessService.GetAllAsync();

        return Ok(businesses);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.BusinessResponse>>> GetActive()
    {
        var businesses = await _businessService.GetActiveAsync();

        return Ok(businesses);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.BusinessResponse>> GetById(int id)
    {
        var business = await _businessService.GetByIdAsync(id);

        if (business == null)
            return NotFound();

        return Ok(business);
    }

    [HttpPost]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<DTOs.Responses.BusinessResponse>> Create(
        CreateBusinessRequest request
    )
    {
        var business = await _businessService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = business.Id }, business);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<DTOs.Responses.BusinessResponse>> Update(
        int id,
        UpdateBusinessRequest request
    )
    {
        var business = await _businessService.UpdateAsync(id, request);

        if (business == null)
            return NotFound();

        return Ok(business);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _businessService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
