using EchoWarehouse.Data;
using EchoWarehouse.Models.DTOs.Auth;
using EchoWarehouse.Repositories.Interfaces;
using Npgsql;

namespace EchoWarehouse.Repositories.Auth;

public class UserRepository : IUserRepository
{
    private readonly EchoWarehouseDbContext _db;

    public UserRepository(EchoWarehouseDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT id, username, email, password_hash, refresh_token, refresh_token_expiry_time,
                   is_active, created_at, updated_at, role
            FROM users
            WHERE username = @username
            LIMIT 1;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", username);

        await using var reader = await command.ExecuteReaderAsync();
        return await ReadUserAsync(reader);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        const string sql = @"
            SELECT id, username, email, password_hash, refresh_token, refresh_token_expiry_time,
                   is_active, created_at, updated_at, role
            FROM users
            WHERE refresh_token = @refresh_token
              AND refresh_token_expiry_time > NOW()
            LIMIT 1;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("refresh_token", refreshToken);

        await using var reader = await command.ExecuteReaderAsync();
        return await ReadUserAsync(reader);
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        const string sql = @"
            SELECT 1
            FROM users
            WHERE username = @username OR email = @email
            LIMIT 1;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", username);
        command.Parameters.AddWithValue("email", email);

        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<int?> CreateUserAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (username, email, password_hash, refresh_token, refresh_token_expiry_time,
                               is_active, created_at, updated_at, role)
            VALUES (@username, @email, @password_hash, NULL, NULL, @is_active, @created_at, NULL, @role)
            RETURNING id;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", user.Username);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("is_active", user.IsActive);
        command.Parameters.AddWithValue("created_at", user.CreatedAt);
        command.Parameters.AddWithValue("role", (object?)user.Role ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync();
        return result is int id ? id : null;
    }

    public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime)
    {
        const string sql = @"
            UPDATE users
            SET refresh_token = @refresh_token,
                refresh_token_expiry_time = @refresh_token_expiry_time,
                updated_at = @updated_at
            WHERE id = @id;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("refresh_token", refreshToken);
        command.Parameters.AddWithValue("refresh_token_expiry_time", refreshTokenExpiryTime);
        command.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
        command.Parameters.AddWithValue("id", userId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> ClearRefreshTokenAsync(int userId)
    {
        const string sql = @"
            UPDATE users
            SET refresh_token = NULL,
                refresh_token_expiry_time = NULL,
                updated_at = @updated_at
            WHERE id = @id;";

        await using var connection = await _db.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
        command.Parameters.AddWithValue("id", userId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static async Task<User?> ReadUserAsync(NpgsqlDataReader reader)
    {
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            RefreshToken = reader.IsDBNull(reader.GetOrdinal("refresh_token")) ? null : reader.GetString(reader.GetOrdinal("refresh_token")),
            RefreshTokenExpiryTime = reader.IsDBNull(reader.GetOrdinal("refresh_token_expiry_time")) ? null : reader.GetDateTime(reader.GetOrdinal("refresh_token_expiry_time")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at")),
            Role = reader.IsDBNull(reader.GetOrdinal("role")) ? null : reader.GetString(reader.GetOrdinal("role"))
        };
    }
}

