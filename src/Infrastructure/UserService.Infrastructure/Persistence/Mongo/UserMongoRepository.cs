using MongoDB.Driver;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Persistence.Mongo;

namespace UserService.Infrastructure.Persistence.Mongo;

public class UserMongoRepository : IUserRepository
{
    private readonly UserMongoContext _context;

    public UserMongoRepository(UserMongoContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _context.Users.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var count = await _context.Users.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        var count = await _context.Users.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}

