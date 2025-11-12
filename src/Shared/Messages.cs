using OpenSleigh.Transport;

namespace Shared;

public record StartOrderSaga(Guid OrderId, Guid CustomerId, decimal TotalAmount) : IMessage;
public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : IMessage;
public record CheckInventoryCommand(Guid OrderId, List<OrderItemMessage> Items) : IMessage;
public record ProcessShippingCommand(Guid OrderId) : IMessage;

public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount) : IMessage;
public record PaymentProcessedEvent(Guid OrderId, bool Success, string? FailureReason = null) : IMessage;
public record InventoryCheckedEvent(Guid OrderId, bool Available, string? FailureReason = null) : IMessage;
public record ShippingProcessedEvent(Guid OrderId, string TrackingNumber) : IMessage;
public record OrderCompletedEvent(Guid OrderId) : IMessage;
public record OrderFailedEvent(Guid OrderId, string Reason) : IMessage;

public record OrderItemMessage(Guid ProductId, string ProductName, int Quantity, decimal Price);

