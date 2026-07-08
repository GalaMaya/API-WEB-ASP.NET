using WebDevToCSharp.Models;

namespace WebDevToCSharp.Services;

public interface IAuthService
{
    string GenerateToken(User user);
}