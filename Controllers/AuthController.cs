using arabia.DTOs.Requests;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace arabia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <returns>Authentication token and user info</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(DTOs.Responses.AuthResponse), 201)]
    [ProducesResponseType(400)]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token and user info</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(DTOs.Responses.AuthResponse), 200)]
    [ProducesResponseType(401)]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(DTOs.Responses.AuthResponse), 200)]
    [ProducesResponseType(401)]
    [AllowAnonymous]
    public async Task<ActionResult<DTOs.Responses.AuthResponse>> RefreshToken(
        [FromBody] string refreshToken
    )
    {
        var response = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(response);
    }

    /// <summary>
    /// Logout and invalidate refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to invalidate</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        await _authService.LogoutAsync(refreshToken);

        return Ok(new { message = "Logged out successfully" });
    }
}
