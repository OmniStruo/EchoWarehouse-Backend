using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EchoWarehouse.Models.DTOs.Auth;
using EchoWarehouse.Services;
using System.Security.Claims;
using EchoWarehouse.Services.Interfaces;

namespace EchoWarehouse.Controllers;

[ApiController]
[Route("api/auth")]
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
    /// <remarks>
    /// Creates a new user account with username, email, and password.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        _logger.LogInformation($"Register attempt for username: {request.Username}");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.RegisterAsync(request);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Created(string.Empty, response);
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <remarks>
    /// Authenticates the user and returns access token and user information.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation($"Login attempt for username: {request.Username}");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(request);
        
        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <remarks>
    /// Uses refresh token to get a new access token without re-entering credentials.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        _logger.LogInformation("Token refresh attempt");
        
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new { success = false });
        }

        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        
        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <remarks>
    /// Invalidates the user's refresh token.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<object>> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "User not found in token" });
        }

        _logger.LogInformation($"Logout for user ID: {userId}");

        var success = await _authService.LogoutAsync(userId);
        
        if (!success)
        {
            return BadRequest(new { message = "Logout failed" });
        }

        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// Validate token
    /// </summary>
    /// <remarks>
    /// Checks if a JWT token is valid.
    /// </remarks>
    [HttpPost("validate")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> ValidateToken([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Token is required" });
        }

        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { valid = isValid });
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    /// <remarks>
    /// Returns information about the currently authenticated user.
    /// </remarks>
    [HttpGet("getCurrentUser")]
    [Authorize]
    public ActionResult<object> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var usernameClaim = User.FindFirst(ClaimTypes.Name);
        var emailClaim = User.FindFirst(ClaimTypes.Email);
        var roleClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null)
        {
            return Unauthorized(new { message = "User not found in token" });
        }

        return Ok(new
        {
            id = userIdClaim.Value,
            username = usernameClaim?.Value,
            email = emailClaim?.Value,
            role = roleClaim?.Value
        });
    }
}



