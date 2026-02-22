using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EchoWarehouse.Extensions.Helpers.Auth;
using Microsoft.IdentityModel.Tokens;
using EchoWarehouse.Models.DTOs.Auth;
using EchoWarehouse.Services.Interfaces;
using EchoWarehouse.Repositories.Interfaces;

namespace EchoWarehouse.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _users;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public AuthService(IConfiguration configuration, IUserRepository users)
    {
        _configuration = configuration;
        _users = users;
        _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "EchoWarehouse";
        _jwtAudience = configuration["Jwt:Audience"] ?? "EchoWarehouseApp";
        _accessTokenExpiryMinutes = int.TryParse(configuration["Jwt:AccessTokenExpiryMinutes"], out var minutes) ? minutes : 15;
        _refreshTokenExpiryDays = int.TryParse(configuration["Jwt:RefreshTokenExpiryDays"], out var days) ? days : 7;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponseDto { Success = false, User = null };
            }

            var user = await _users.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive || !AuthHelpers.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new LoginResponseDto { Success = false, User = null };
            }

            var accessToken = AuthHelpers.GenerateAccessToken(user, _jwtSecret, _jwtIssuer, _jwtAudience, _accessTokenExpiryMinutes);
            var refreshToken = AuthHelpers.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);

            await _users.UpdateRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));

            return new LoginResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = AuthHelpers.MapUserToDto(user)
            };
        }
        catch
        {
            return new LoginResponseDto { Success = false, User = null };
        }
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new RegisterResponseDto { Success = false };
            }

            if (request.Password != request.PasswordConfirm)
            {
                return new RegisterResponseDto { Success = false };
            }

            if (request.Password.Length < 6)
            {
                return new RegisterResponseDto { Success = false };
            }

            if (await _users.UserExistsAsync(request.Username, request.Email))
            {
                return new RegisterResponseDto { Success = false };
            }

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = AuthHelpers.HashPassword(request.Password),
                IsActive = true,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            var newUserId = await _users.CreateUserAsync(newUser);
            if (newUserId == null)
            {
                return new RegisterResponseDto { Success = false };
            }

            newUser.Id = newUserId.Value;

            return new RegisterResponseDto
            {
                Success = true,
                User = AuthHelpers.MapUserToDto(newUser)
            };
        }
        catch
        {
            return new RegisterResponseDto { Success = false };
        }
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new RefreshTokenResponseDto { Success = false };
            }

            var user = await _users.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return new RefreshTokenResponseDto { Success = false };
            }

            var newAccessToken = AuthHelpers.GenerateAccessToken(user, _jwtSecret, _jwtIssuer, _jwtAudience, _accessTokenExpiryMinutes);
            var newRefreshToken = AuthHelpers.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);

            await _users.UpdateRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));

            return new RefreshTokenResponseDto
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            };
        }
        catch
        {
            return new RefreshTokenResponseDto { Success = false };
        }
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        try
        {
            return await _users.ClearRefreshTokenAsync(userId);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken is JwtSecurityToken;
        }
        catch
        {
            return false;
        }
    }
}