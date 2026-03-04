using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EchoWarehouse.Models.DTOs.Auth;
using System.Security.Claims;
using EchoWarehouse.Services.Auth.Interfaces;
using EchoWarehouse.Extensions.Helpers;
using EchoWarehouse.Models.DTOs.Common;
using EchoWarehouse.Validators;

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
    public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
    {
        _logger.LogInformation($"Register attempt for username: {request.Username}");
        
        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(new ApiErrorDto(
                message: result.Message ?? ValidationErrorKeys.UnexpectedError,
                status: 400,
                details: result.Details
            ));
        }

        return Created(string.Empty, result.Data);
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <remarks>
    /// Authenticates the user and returns access token and user information.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation($"Login attempt for username: {request.Username}");
        
        var result = await _authService.LoginAsync(request);
        
        if (!result.Success)
        {
            return Unauthorized(new ApiErrorDto(
                message: result.Message ?? ValidationErrorKeys.UnexpectedError,
                status: 401,
                details: result.Details
            ));
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <remarks>
    /// Uses refresh token to get a new access token without re-entering credentials.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        _logger.LogInformation("Token refresh attempt");
   
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        
        if (!result.Success)
        {
            return Unauthorized(new ApiErrorDto(
                message: result.Message ?? ValidationErrorKeys.UnexpectedError,
                status: 401,
                details: result.Details
            ));
        }

        return Ok(result.Data);
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
            return ErrorResponseHelper.UnauthorizedError(ValidationErrorKeys.UserNotFound);
        }

        _logger.LogInformation($"Logout for user ID: {userId}");

        var success = await _authService.LogoutAsync(userId);
        
        if (!success)
        {
            return ErrorResponseHelper.BadRequestError(ValidationErrorKeys.FailedToLogoutUser);
        }

        return Ok();
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
            return ErrorResponseHelper.BadRequestError(ValidationErrorKeys.RequiredField);
        }

        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { valid = isValid });
    }
}



