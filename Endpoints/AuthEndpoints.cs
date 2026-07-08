using WebDevToCSharp.Models;
using WebDevToCSharp.Models.Dtos;
using WebDevToCSharp.Services;

namespace WebDevToCSharp.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", (RegisterDto dto, IUserRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return Results.BadRequest(new { message = "Email and password required" });

            if (repo.GetByEmail(dto.Email) != null)
                return Results.Conflict(new { message = "Email already registered" });

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            repo.Create(user);
            return Results.Ok(new { message = "User registered successfully" });
        });

        app.MapPost("/api/auth/login", (LoginDto dto, IUserRepository repo, IAuthService auth) =>
        {
            var user = repo.GetByEmail(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Results.Unauthorized();

            var token = auth.GenerateToken(user);
            var userDto = new UserDto(user.Id, user.Username, user.Email, user.CreatedAt);
            return Results.Ok(new AuthResponseDto(token, userDto));
        });
    }
}