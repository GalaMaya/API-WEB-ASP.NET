using WebDevToCSharp.Models;

namespace WebDevToCSharp.Services;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();
    private int _nextId = 1;

    public User? GetByEmail(string email)
        => _users.Values.FirstOrDefault(u => u.Email == email);

    public User? GetById(int id)
        => _users.TryGetValue(id, out var user) ? user : null;

    public IEnumerable<User> GetAll() => _users.Values;

    public User Create(User user)
    {
        user.Id = _nextId++;
        _users[user.Id] = user;
        return user;
    }
}