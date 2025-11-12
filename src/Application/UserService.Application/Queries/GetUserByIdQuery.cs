using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

