using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserReadRepository _readRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserReadRepository readRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _readRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", request.UserId);
            return null;
        }

        return user;
    }
}

