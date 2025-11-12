using Microsoft.Extensions.Logging;
using OpenSleigh;
using OpenSleigh.Transport;
using Shared;

namespace OrderService.Orchestrator.Sagas;

public record OrderSagaState
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool PaymentProcessed { get; set; } = false;
    public bool InventoryChecked { get; set; } = false;
    public bool ShippingProcessed { get; set; } = false;
}

public class OrderSaga :
    Saga<OrderSagaState>,
    IStartedBy<StartOrderSaga>,
    IHandleMessage<PaymentProcessedEvent>,
    IHandleMessage<InventoryCheckedEvent>,
    IHandleMessage<ShippingProcessedEvent>
{
    private readonly ILogger<OrderSaga> _logger;

    public OrderSaga(
        ISagaInstance<OrderSagaState> context,
        ILogger<OrderSaga> logger) : base(context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask HandleAsync(IMessageContext<StartOrderSaga> context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting order saga for OrderId: {OrderId}", context.Message.OrderId);

        this.Context.State.OrderId = context.Message.OrderId;
        this.Context.State.CustomerId = context.Message.CustomerId;
        this.Context.State.TotalAmount = context.Message.TotalAmount;

        var processPayment = new ProcessPaymentCommand(context.Message.OrderId, context.Message.TotalAmount);
        this.Publish(processPayment);

        var checkInventory = new CheckInventoryCommand(context.Message.OrderId, new List<OrderItemMessage>());
        this.Publish(checkInventory);
    }

    public async ValueTask HandleAsync(IMessageContext<PaymentProcessedEvent> context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Payment processed for OrderId: {OrderId}, Success: {Success}",
            context.Message.OrderId, context.Message.Success);

        if (!context.Message.Success)
        {
            _logger.LogError("Payment failed for OrderId: {OrderId}, Reason: {Reason}",
                context.Message.OrderId, context.Message.FailureReason);
            
            var failedEvent = new OrderFailedEvent(context.Message.OrderId, 
                $"Payment failed: {context.Message.FailureReason}");
            this.Publish(failedEvent);
            this.Context.MarkAsCompleted();
            return;
        }

        this.Context.State.PaymentProcessed = true;

        if (CanProceedToShipping())
        {
            await ProceedToShipping(cancellationToken);
        }
    }

    public async ValueTask HandleAsync(IMessageContext<InventoryCheckedEvent> context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Inventory checked for OrderId: {OrderId}, Available: {Available}",
            context.Message.OrderId, context.Message.Available);

        if (!context.Message.Available)
        {
            _logger.LogError("Inventory check failed for OrderId: {OrderId}, Reason: {Reason}",
                context.Message.OrderId, context.Message.FailureReason);
            
            var failedEvent = new OrderFailedEvent(context.Message.OrderId,
                $"Inventory check failed: {context.Message.FailureReason}");
            this.Publish(failedEvent);
            this.Context.MarkAsCompleted();
            return;
        }

        this.Context.State.InventoryChecked = true;

        if (CanProceedToShipping())
        {
            await ProceedToShipping(cancellationToken);
        }
    }

    public async ValueTask HandleAsync(IMessageContext<ShippingProcessedEvent> context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Shipping processed for OrderId: {OrderId}, TrackingNumber: {TrackingNumber}",
            context.Message.OrderId, context.Message.TrackingNumber);

        this.Context.State.ShippingProcessed = true;

        var completedEvent = new OrderCompletedEvent(context.Message.OrderId);
        this.Publish(completedEvent);

        _logger.LogInformation("Order saga completed successfully for OrderId: {OrderId}", context.Message.OrderId);
        this.Context.MarkAsCompleted();
    }

    private bool CanProceedToShipping()
    {
        return this.Context.State.PaymentProcessed && 
               this.Context.State.InventoryChecked && 
               !this.Context.State.ShippingProcessed;
    }

    private async ValueTask ProceedToShipping(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Proceeding to shipping for OrderId: {OrderId}", this.Context.State.OrderId);
        
        var shippingCommand = new ProcessShippingCommand(this.Context.State.OrderId);
        this.Publish(shippingCommand);
    }
}

