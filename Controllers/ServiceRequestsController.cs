using arabia.DTOs.Requests;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace arabia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly IServiceRequestService _serviceRequestService;
    private readonly ILogger<ServiceRequestsController> _logger;

    public ServiceRequestsController(
        IServiceRequestService serviceRequestService,
        ILogger<ServiceRequestsController> logger
    )
    {
        _serviceRequestService = serviceRequestService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.ServiceRequestResponse>>> GetAll()
    {
        var serviceRequests = await _serviceRequestService.GetAllAsync();

        return Ok(serviceRequests);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<DTOs.Responses.ServiceRequestResponse>> GetById(int id)
    {
        var serviceRequest = await _serviceRequestService.GetByIdAsync(id);

        if (serviceRequest == null)
            return NotFound();

        return Ok(serviceRequest);
    }

    [HttpGet("business/{businessId}")]
    [Authorize(Policy = "User")]
    public async Task<
        ActionResult<IEnumerable<DTOs.Responses.ServiceRequestResponse>>
    > GetByBusinessId(int businessId)
    {
        var serviceRequests = await _serviceRequestService.GetByBusinessIdAsync(businessId);

        return Ok(serviceRequests);
    }

    [HttpGet("employee/{employeeId}")]
    [Authorize(Policy = "User")]
    public async Task<
        ActionResult<IEnumerable<DTOs.Responses.ServiceRequestResponse>>
    > GetByEmployeeId(int employeeId)
    {
        var serviceRequests = await _serviceRequestService.GetByEmployeeIdAsync(employeeId);

        return Ok(serviceRequests);
    }

    [HttpGet("status/{status}")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.ServiceRequestResponse>>> GetByStatus(
        string status
    )
    {
        var serviceRequests = await _serviceRequestService.GetByStatusAsync(status);

        return Ok(serviceRequests);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.ServiceRequestResponse>> Create(
        CreateServiceRequestRequest request
    )
    {
        var serviceRequest = await _serviceRequestService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, serviceRequest);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<DTOs.Responses.ServiceRequestResponse>> Update(
        int id,
        UpdateServiceRequestRequest request
    )
    {
        var serviceRequest = await _serviceRequestService.UpdateAsync(id, request);

        if (serviceRequest == null)
            return NotFound();

        return Ok(serviceRequest);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _serviceRequestService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
