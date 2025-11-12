using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProjectionService _projectionService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUserProjectionService projectionService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _projectionService = projectionService;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        var user = new User(request.Email, request.FirstName, request.LastName);
        
        await _userRepository.AddAsync(user, cancellationToken);
        
        await _projectionService.ProjectAsync(user, cancellationToken);
        
        _logger.LogInformation("User {UserId} created successfully and projected to read model", user.Id);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}

