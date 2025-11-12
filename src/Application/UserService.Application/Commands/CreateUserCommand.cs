using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : IRequest<UserDto>;

