using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;
using System.Net;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
);

