using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using System.Net;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.FirstName,
            request.LastName
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

public record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName
);

