using EchoWarehouse.Models.DTOs.Auth;
using EchoWarehouse.Models.DTOs.Common;

namespace EchoWarehouse.Services.Auth.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(int userId);
    Task<bool> ValidateTokenAsync(string token);
}