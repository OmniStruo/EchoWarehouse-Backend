using EchoWarehouse.Models.DTOs.Auth;

namespace EchoWarehouse.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> UserExistsAsync(string username, string email);
    Task<int?> CreateUserAsync(User user);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime);
    Task<bool> ClearRefreshTokenAsync(int userId);
}

