using WebDevToCSharp.Models;

namespace WebDevToCSharp.Services;

public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetById(int id);
    IEnumerable<User> GetAll();
    User Create(User user);
}