using arabia.DTOs.Requests;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace arabia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        ILogger<EmployeesController> logger
    )
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    /// <returns>List of all employees</returns>
    [HttpGet]
    [Authorize(Policy = "Employee")]
    [ProducesResponseType(typeof(IEnumerable<DTOs.Responses.EmployeeResponse>), 200)]
    public async Task<ActionResult<IEnumerable<DTOs.Responses.EmployeeResponse>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();

        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Employee details</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "Employee")]
    [ProducesResponseType(typeof(DTOs.Responses.EmployeeResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DTOs.Responses.EmployeeResponse>> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpGet("specialization/{specialization}")]
    [Authorize(Policy = "Employee")]
    public async Task<
        ActionResult<IEnumerable<DTOs.Responses.EmployeeResponse>>
    > GetBySpecialization(string specialization)
    {
        var employees = await _employeeService.GetBySpecializationAsync(specialization);

        return Ok(employees);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    /// <param name="request">Employee creation data</param>
    /// <returns>Created employee</returns>
    [HttpPost]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(DTOs.Responses.EmployeeResponse), 201)]
    public async Task<ActionResult<DTOs.Responses.EmployeeResponse>> Create(
        CreateEmployeeRequest request
    )
    {
        var employee = await _employeeService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<DTOs.Responses.EmployeeResponse>> Update(
        int id,
        UpdateEmployeeRequest request
    )
    {
        var employee = await _employeeService.UpdateAsync(id, request);

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
