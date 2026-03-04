using System.ComponentModel.DataAnnotations;
using EchoWarehouse.Models.DTOs.Common;
using EchoWarehouse.Validators;

namespace EchoWarehouse.Models.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto? User { get; set; }
}

public class RegisterRequestDto
{
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    [Compare("Password", ErrorMessage = ValidationErrorKeys.PasswordMismatch)]
    public string PasswordConfirm { get; set; } = string.Empty;
}

public class RegisterResponseDto
{
    public UserDto? User { get; set; }
}

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = ValidationErrorKeys.RequiredField)]
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}




