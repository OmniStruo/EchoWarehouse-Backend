using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EchoWarehouse.Extensions.Helpers.Auth;
using Microsoft.IdentityModel.Tokens;
using EchoWarehouse.Models.DTOs.Auth;
using EchoWarehouse.Models.DTOs.Common;
using EchoWarehouse.Services.Auth.Interfaces;
using EchoWarehouse.Repositories.Interfaces;
using EchoWarehouse.Validators;

namespace EchoWarehouse.Services.Auth;

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

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _users.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive || !AuthHelpers.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result<LoginResponseDto>.ErrorResult(ValidationErrorKeys.InvalidCredentials);
            }

            var accessToken = AuthHelpers.GenerateAccessToken(user, _jwtSecret, _jwtIssuer, _jwtAudience, _accessTokenExpiryMinutes);
            var refreshToken = AuthHelpers.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);

            await _users.UpdateRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));

            return Result<LoginResponseDto>.SuccessResult(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = AuthHelpers.MapUserToDto(user)
            });
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.ErrorResult(ValidationErrorKeys.UnexpectedError);
        }
    }

    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            if (await _users.UserExistsAsync(request.Username, request.Email))
            {
                return Result<RegisterResponseDto>.ErrorResult(
                    ValidationErrorKeys.UserAlreadyExists
                );
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
                return Result<RegisterResponseDto>.ErrorResult(ValidationErrorKeys.FailedToCreateUser);
            }

            newUser.Id = newUserId.Value;

            return Result<RegisterResponseDto>.SuccessResult(new RegisterResponseDto
            {
                User = AuthHelpers.MapUserToDto(newUser)
            });
        }
        catch (Exception ex)
        {
            return Result<RegisterResponseDto>.ErrorResult(ValidationErrorKeys.UnexpectedError);
        }
    }

    public async Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {

            var user = await _users.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return Result<RefreshTokenResponseDto>.ErrorResult(ValidationErrorKeys.UserNotFound);
            }

            var newAccessToken = AuthHelpers.GenerateAccessToken(user, _jwtSecret, _jwtIssuer, _jwtAudience, _accessTokenExpiryMinutes);
            var newRefreshToken = AuthHelpers.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);

            await _users.UpdateRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));

            return Result<RefreshTokenResponseDto>.SuccessResult(new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex)
        {
            return Result<RefreshTokenResponseDto>.ErrorResult(ValidationErrorKeys.UnexpectedError);
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