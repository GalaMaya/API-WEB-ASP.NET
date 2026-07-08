using WebDevToCSharp.Models.Dtos;
using WebDevToCSharp.Services;

namespace WebDevToCSharp.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users")
            .RequireAuthorization(); // 🔒 Middleware auth di sini!

        group.MapGet("/", (IUserRepository repo) =>
        {
            var users = repo.GetAll().Select(u =>
                new UserDto(u.Id, u.Username, u.Email, u.CreatedAt));
            return Results.Ok(users);
        });
    }
}