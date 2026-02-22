using EchoWarehouse.Models.DTOs.Auth;

namespace EchoWarehouse.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(int userId);
    Task<bool> ValidateTokenAsync(string token);
}