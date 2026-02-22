using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using EchoWarehouse.Data;
using EchoWarehouse.Services;
using EchoWarehouse.Services.Interfaces;
using EchoWarehouse.Repositories.Auth;
using EchoWarehouse.Repositories.Interfaces;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key-change-this-in-production";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EchoWarehouse",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EchoWarehouseApp",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Register database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection is not configured.");
}

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.AddScoped<EchoWarehouseDbContext>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();