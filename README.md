# Microservices Architecture with OpenSleigh

A comprehensive microservices architecture implementation using **OpenSleigh** for distributed saga orchestration, following **Domain-Driven Design (DDD)** principles and **CQRS** pattern.

## Architecture Overview

The solution consists of **3 microservices** and an **API Gateway**, each following an **n-layer architecture**:

### Microservices

1. **Order Service** - Manages orders and order processing
2. **Product Service** - Manages product catalog and inventory
3. **User Service** - Manages user accounts and profiles
4. **API Gateway** - Single entry point for all client requests

### Architecture Layers (per microservice)

1. **Domain Layer** - Entities, Value Objects, Domain Events, Repository interfaces
2. **Application Layer** - CQRS (Commands, Queries, Handlers), DTOs, Validation
3. **Infrastructure Layer** - Entity Framework Core, Redis, Repository implementations
4. **API Layer** - RESTful HTTP API, Swagger/OpenAPI documentation
5. **Orchestrator Service** - OpenSleigh saga orchestration for distributed transactions

## Features

### ✅ Domain-Driven Design (DDD)
- Rich domain models with entities and value objects
- Domain events for decoupling
- Repository pattern for data access abstraction

### ✅ CQRS (Command Query Responsibility Segregation)
- Separate command and query models
- Command handlers for write operations
- Query handlers for read operations
- Read models optimized for queries

### ✅ Distributed Saga Pattern
- OpenSleigh for saga orchestration
- State management for long-running processes
- Compensation support for failure scenarios

### ✅ Communication Patterns

**Synchronous:**
- HTTP REST API for client interactions
- API Gateway (YARP) for routing
- gRPC support (infrastructure ready)

**Asynchronous:**
- RabbitMQ for message-based communication
- Event-driven architecture
- Pub/Sub pattern

### ✅ Design Patterns Implemented
- **Repository Pattern** - Data access abstraction
- **CQRS Pattern** - Command/Query separation
- **Saga Pattern** - Distributed transaction orchestration
- **API Gateway Pattern** - Single entry point
- **Domain Events** - Event-driven communication
- **Dependency Injection** - Loose coupling
- **Factory Pattern** - Object creation
- **Strategy Pattern** - Algorithm selection

### ✅ Additional Features
- Idempotency support
- Error handling and retry mechanisms
- **MongoDB** for Commands (Write Database)
- **SQL Server** for Queries (Read Database)
- **Projection Services** to sync data between databases
- Optimized read models for query performance
- Entity Framework Core for SQL Server
- MongoDB Driver for document storage
- Redis for caching and distributed state
- Docker Compose setup
- Comprehensive documentation

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- SQL Server (or use Docker)
- RabbitMQ (or use Docker)
- Redis (or use Docker)

**Note:** This solution uses the OpenSleigh framework from the local `OpenSleigh` directory. Make sure the OpenSleigh projects are built before building this solution.

## Getting Started

### 1. Start Infrastructure Services

```bash
docker-compose up -d
```

This will start:
- SQL Server on port 1433
- MongoDB on port 27017
- RabbitMQ on port 5672 (Management UI on 15672)
- Redis on port 6379

### 2. Build OpenSleigh Framework

First, build the OpenSleigh framework projects:

```bash
cd OpenSleigh/src
dotnet build
```

### 3. Configure Connection Strings

Update `appsettings.json` files in:
- `src/API/OrderService.API/appsettings.json`
- `src/API/ProductService.API/appsettings.json`
- `src/API/UserService.API/appsettings.json`
- `src/Services/OrderService.Orchestrator/appsettings.json`

The default connection strings are configured for Docker containers. Adjust if needed.

### 4. Build the Solution

```bash
dotnet build MicroservicesArchitecture.sln
```

### 5. Run Database Migrations

**Note:** MongoDB doesn't require migrations. Only SQL Server read databases need migrations.

The APIs will automatically run SQL Server migrations on startup. Alternatively:

```bash
# Order Service (SQL Server Read DB)
cd src/API/OrderService.API
dotnet ef database update --context OrderReadDbContext

# Product Service (SQL Server Read DB)
cd src/API/ProductService.API
dotnet ef database update --context ProductReadDbContext

# User Service (SQL Server Read DB)
cd src/API/UserService.API
dotnet ef database update --context UserReadDbContext
```

**Note:** MongoDB databases are created automatically when first accessed.

### 6. Run the Services

**Terminal 1 - API Gateway:**
```bash
cd src/API/Gateway.API
dotnet run
```

**Terminal 2 - Order Service:**
```bash
cd src/API/OrderService.API
dotnet run
```

**Terminal 3 - Product Service:**
```bash
cd src/API/ProductService.API
dotnet run
```

**Terminal 4 - User Service:**
```bash
cd src/API/UserService.API
dotnet run
```

**Terminal 5 - Orchestrator:**
```bash
cd src/Services/OrderService.Orchestrator
dotnet run
```

### 7. Access the Services

- **API Gateway:** `http://localhost:5001`
- **Order Service:** `http://localhost:5000/swagger`
- **Product Service:** `http://localhost:5002/swagger`
- **User Service:** `http://localhost:5003/swagger`
- **RabbitMQ Management:** `http://localhost:15672` (guest/guest)

## API Endpoints (via Gateway)

### Order Service

**Create Order:**
```http
POST http://localhost:5001/api/orders
Content-Type: application/json
Idempotency-Key: <optional-unique-key>

{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "items": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "productName": "Product 1",
      "price": 29.99,
      "quantity": 2
    }
  ]
}
```

**Get Order by ID:**
```http
GET http://localhost:5001/api/orders/{id}
```

**Get Orders by Customer:**
```http
GET http://localhost:5001/api/orders/customer/{customerId}
```

### Product Service

**Create Product:**
```http
POST http://localhost:5001/api/products
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product Description",
  "price": 29.99,
  "stockQuantity": 100
}
```

**Get Product by ID:**
```http
GET http://localhost:5001/api/products/{id}
```

### User Service

**Create User:**
```http
POST http://localhost:5001/api/users
Content-Type: application/json

{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Get User by ID:**
```http
GET http://localhost:5001/api/users/{id}
```

## Architecture Patterns

### Saga Orchestration Flow

1. **Order Created** → API Gateway receives request
2. **Order Service** → Creates order entity
3. **Saga Started** → OpenSleigh creates OrderSaga instance
4. **Parallel Processing:**
   - Payment processing
   - Inventory check
5. **Shipping** → Triggered when both payment and inventory succeed
6. **Completion** → Order marked as completed

### Message Flow

```
Gateway → Order Service → CreateOrderCommand → RabbitMQ → Orchestrator (OrderSaga)
OrderSaga → ProcessPaymentCommand → Payment Service
OrderSaga → CheckInventoryCommand → Inventory Service
Payment Service → PaymentProcessedEvent → OrderSaga
Inventory Service → InventoryCheckedEvent → OrderSaga
OrderSaga → ProcessShippingCommand → Shipping Service
Shipping Service → ShippingProcessedEvent → OrderSaga
OrderSaga → OrderCompletedEvent → All Services
```

## Technology Stack

- **.NET 8.0** - Runtime and framework
- **OpenSleigh** - Saga orchestration
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Entity Framework Core** - ORM
- **SQL Server** - Primary database
- **Redis** - Caching and distributed state
- **RabbitMQ** - Message broker
- **YARP** - API Gateway (Reverse Proxy)
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
MicroservicesArchitecture/
├── src/
│   ├── Domain/
│   │   ├── OrderService.Domain/
│   │   ├── ProductService.Domain/
│   │   └── UserService.Domain/
│   ├── Application/
│   │   ├── OrderService.Application/
│   │   ├── ProductService.Application/
│   │   └── UserService.Application/
│   ├── Infrastructure/
│   │   ├── OrderService.Infrastructure/
│   │   ├── ProductService.Infrastructure/
│   │   └── UserService.Infrastructure/
│   ├── API/
│   │   ├── OrderService.API/
│   │   ├── ProductService.API/
│   │   ├── UserService.API/
│   │   └── Gateway.API/
│   ├── Services/
│   │   └── OrderService.Orchestrator/
│   └── Shared/
│       └── Messages.cs
├── docker-compose.yml
├── README.md
└── ARCHITECTURE.md
```

## Design Patterns Summary

### 1. Repository Pattern
- Abstraction over data access
- Domain-focused interfaces
- Infrastructure implementations

### 2. CQRS Pattern
- Separate read and write models
- Command handlers for writes
- Query handlers for reads
- Optimized read models

### 3. Saga Pattern
- Distributed transaction management
- State persistence
- Compensation support
- Long-running processes

### 4. API Gateway Pattern
- Single entry point
- Request routing
- Load balancing ready
- Service discovery ready

### 5. Domain Events Pattern
- Decoupled communication
- Event-driven architecture
- Event sourcing ready

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
✅ API Gateway Pattern  
✅ Idempotency  
✅ Error Handling  
✅ Logging  
✅ Configuration Management  

## Extending the Architecture

### Adding a New Microservice

1. Create domain entities and value objects
2. Add commands/queries in Application layer
3. Implement infrastructure (repository, external services)
4. Create API controllers
5. Add route in Gateway configuration
6. Add saga handlers in Orchestrator if needed

### Adding gRPC Support

1. Define `.proto` files
2. Generate C# code using `Grpc.Tools`
3. Implement service classes
4. Register in `Program.cs`

### Adding MongoDB Support

Replace EF Core with MongoDB driver:
- Update Infrastructure projects
- Change repository implementations
- Update OpenSleigh configuration to use MongoDB persistence

## CI/CD

The solution is ready for CI/CD integration:
- GitHub Actions workflows can be added
- Docker images can be built for each service
- Kubernetes manifests can be created for deployment

## License

This is a sample implementation for educational purposes.

## Contributing

Feel free to extend this architecture with:
- Additional microservices
- More complex saga patterns
- Event sourcing
- Service mesh integration
- Authentication/Authorization
- Rate limiting
- Circuit breakers
