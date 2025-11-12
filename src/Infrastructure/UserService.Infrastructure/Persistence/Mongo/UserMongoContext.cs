using MongoDB.Driver;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Mongo;

public class UserMongoContext
{
    private readonly IMongoDatabase _database;

    public UserMongoContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
}

