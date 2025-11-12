# Architecture Documentation

## Overview

This microservices architecture implements a comprehensive n-layer design pattern using OpenSleigh for distributed saga orchestration, following Domain-Driven Design (DDD) principles and CQRS pattern.

## Architecture Layers

### 1. Domain Layer (`OrderService.Domain`)

**Purpose:** Contains the core business logic and domain models.

**Components:**
- **Entities:** `Order`, `OrderItem` - Rich domain models with business logic
- **Value Objects:** `CustomerId`, `ProductId`, `Money` - Immutable value types
- **Domain Events:** `OrderCreatedEvent`, `OrderCompletedEvent`, etc. - Represent domain state changes
- **Repository Interfaces:** `IOrderRepository` - Abstraction for data access

**Key Principles:**
- No infrastructure dependencies
- Rich domain models with behavior
- Domain events for decoupling
- Repository pattern for abstraction

### 2. Application Layer (`OrderService.Application`)

**Purpose:** Orchestrates domain objects to perform application tasks.

**Components:**
- **Commands:** `CreateOrderCommand` - Write operations
- **Queries:** `GetOrderByIdQuery`, `GetOrdersByCustomerQuery` - Read operations
- **Handlers:** Command and Query handlers using MediatR
- **DTOs:** Data Transfer Objects for API communication
- **Validation:** FluentValidation for input validation
- **Interfaces:** Application service interfaces

**CQRS Implementation:**
- Commands modify state (write operations)
- Queries read data (read operations)
- Separate models for read and write
- MediatR for request/response handling

### 3. Infrastructure Layer (`OrderService.Infrastructure`)

**Purpose:** Implements technical concerns and external integrations.

**Components:**
- **Persistence:** Entity Framework Core with SQL Server
- **Repositories:** Concrete implementations of domain repositories
- **Caching:** Redis for distributed caching
- **Idempotency:** Redis-based idempotency service
- **External Services:** Integration with third-party services

**Technologies:**
- Entity Framework Core 8.0
- SQL Server
- Redis (StackExchange.Redis)
- OpenSleigh (for messaging)

### 4. API Layer (`OrderService.API`)

**Purpose:** Exposes application functionality via HTTP/gRPC endpoints.

**Components:**
- **Controllers:** RESTful API endpoints
- **OpenAPI/Swagger:** API documentation
- **Message Publishing:** Integration with OpenSleigh message bus
- **Middleware:** Request/response pipeline

**Endpoints:**
- `POST /api/orders` - Create order
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/customer/{customerId}` - Get orders by customer

### 5. Orchestrator Service (`OrderService.Orchestrator`)

**Purpose:** Coordinates distributed transactions using OpenSleigh sagas.

**Components:**
- **Sagas:** `OrderSaga` - Orchestrates order processing workflow
- **State Management:** Saga state persistence
- **Message Handling:** Handles events from other services

**Saga Flow:**
1. Order created → Start saga
2. Parallel processing:
   - Payment processing
   - Inventory check
3. Shipping → Triggered when both succeed
4. Completion → Order marked as completed

### 6. Shared (`Shared`)

**Purpose:** Common contracts and messages shared between services.

**Components:**
- **Messages:** OpenSleigh message contracts
- **DTOs:** Shared data transfer objects

## Communication Patterns

### Synchronous Communication

**HTTP REST API:**
- Client-to-service communication
- Request/response pattern
- Stateless interactions

**gRPC (Ready for Implementation):**
- Service-to-service communication
- High performance
- Strongly typed contracts

### Asynchronous Communication

**RabbitMQ:**
- Message-based communication
- Event-driven architecture
- Pub/Sub pattern
- Decoupled services

**OpenSleigh:**
- Saga orchestration
- State management
- Distributed transaction coordination

## Data Flow

### Order Creation Flow

```
1. Client → HTTP POST /api/orders
2. API Controller → MediatR Command Handler
3. Command Handler → Domain Entity (Order)
4. Domain Entity → Repository (Persistence)
5. API → OpenSleigh Message Bus (StartOrderSaga)
6. RabbitMQ → Orchestrator Service
7. OrderSaga → Process Payment & Inventory (Parallel)
8. Services → Events back to Saga
9. Saga → Process Shipping
10. Saga → Order Completed
```

### Saga Orchestration Flow

```
OrderSaga State Machine:
- Initial: Order Created
- Payment Processed: PaymentProcessedEvent
- Inventory Checked: InventoryCheckedEvent
- Shipping Processed: ShippingProcessedEvent
- Final: Order Completed
```

## Design Patterns

### 1. Domain-Driven Design (DDD)
- Rich domain models
- Value objects
- Domain events
- Aggregate roots
- Repository pattern

### 2. CQRS (Command Query Responsibility Segregation)
- Separate read and write models
- Command handlers for writes
- Query handlers for reads
- Optimized read models

### 3. Saga Pattern
- Distributed transaction management
- State persistence
- Compensation support
- Long-running processes

### 4. Repository Pattern
- Abstraction over data access
- Domain-focused interfaces
- Infrastructure implementations

### 5. Dependency Injection
- Constructor injection
- Interface-based design
- Loose coupling

## Error Handling & Resilience

### Idempotency
- Idempotency keys for operations
- Redis-based storage
- Prevents duplicate processing

### Retry Policies
- Built into OpenSleigh
- Configurable retry strategies
- Exponential backoff

### Compensation
- Saga compensation for rollback
- Error event handling
- State recovery

## Scalability Considerations

### Horizontal Scaling
- Stateless API services
- Message queue for load distribution
- Database connection pooling

### Caching Strategy
- Redis for distributed cache
- Query result caching
- Idempotency key storage

### Database
- SQL Server for transactional data
- Connection pooling
- Index optimization ready

## Security Considerations

### API Security
- HTTPS in production
- Authentication/Authorization (ready for implementation)
- Input validation

### Message Security
- RabbitMQ authentication
- Secure connection strings
- Environment-based configuration

## Testing Strategy

### Unit Tests
- Domain logic
- Application handlers
- Value objects

### Integration Tests
- Repository implementations
- API endpoints
- Saga orchestration

### E2E Tests
- Complete workflows
- Service interactions
- Message flow

## Deployment

### Docker Support
- `docker-compose.yml` for infrastructure
- Container-ready services
- Environment configuration

### CI/CD Ready
- Solution structure supports CI/CD
- Build scripts ready
- Deployment manifests (to be added)

## Future Enhancements

1. **API Gateway:** Centralized entry point
2. **Service Mesh:** Advanced service communication
3. **Event Sourcing:** Complete event history
4. **GraphQL:** Flexible query interface
5. **Monitoring:** Application insights, logging
6. **Authentication:** OAuth2, JWT implementation
7. **Rate Limiting:** API protection
8. **Circuit Breaker:** Resilience patterns

## Technology Stack Summary

| Layer | Technology |
|-------|-----------|
| Domain | Pure C# |
| Application | MediatR, FluentValidation |
| Infrastructure | EF Core, Redis, SQL Server |
| API | ASP.NET Core, Swagger |
| Messaging | RabbitMQ, OpenSleigh |
| Orchestration | OpenSleigh Sagas |
| Caching | Redis |
| Database | SQL Server |

## Best Practices Implemented

✅ Separation of Concerns  
✅ Dependency Inversion  
✅ Single Responsibility  
✅ Open/Closed Principle  
✅ Interface Segregation  
✅ Don't Repeat Yourself (DRY)  
✅ SOLID Principles  
✅ Clean Architecture  
✅ Domain-Driven Design  
✅ CQRS Pattern  
✅ Saga Pattern  
✅ Repository Pattern  
✅ Idempotency  
✅ Error Handling  
✅ Logging  
✅ Configuration Management  

