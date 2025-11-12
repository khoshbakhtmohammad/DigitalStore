using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using System.Text.Json;

namespace OrderService.Infrastructure.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyService> _logger;
    private const string KeyPrefix = "idempotency:order:";

    public IdempotencyService(
        IDistributedCache cache,
        ILogger<IdempotencyService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<OrderDto?> GetOrderByKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}{idempotencyKey}";
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(cachedData))
                return null;

            var orderId = JsonSerializer.Deserialize<Guid>(cachedData);
            if (orderId == Guid.Empty)
                return null;

            _logger.LogInformation("Found idempotent order {OrderId} for key {Key}", orderId, idempotencyKey);
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving idempotency key {Key}", idempotencyKey);
            return null;
        }
    }

    public async Task StoreIdempotencyKeyAsync(string idempotencyKey, Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}{idempotencyKey}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            var data = JsonSerializer.Serialize(orderId);
            await _cache.SetStringAsync(key, data, options, cancellationToken);
            
            _logger.LogInformation("Stored idempotency key {Key} for order {OrderId}", idempotencyKey, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing idempotency key {Key}", idempotencyKey);
        }
    }
}

