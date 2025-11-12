# CQRS Implementation Guide

## Overview

This architecture implements **true CQRS (Command Query Responsibility Segregation)** with:
- **MongoDB** for Commands (Write Database)
- **SQL Server** for Queries (Read Database)
- **Projection Services** to sync data from MongoDB to SQL Server

## Architecture Pattern

### Command Side (Write)
- **Database:** MongoDB
- **Purpose:** Store domain entities for write operations
- **Repository:** `*MongoRepository` (e.g., `OrderMongoRepository`)
- **Operations:** Create, Update, Delete

### Query Side (Read)
- **Database:** SQL Server
- **Purpose:** Optimized read models for query operations
- **Repository:** `*ReadRepository` (e.g., `OrderReadRepository`)
- **Operations:** GetById, GetByCustomer, GetAll, etc.

### Projection Layer
- **Service:** `*ProjectionService` (e.g., `OrderProjectionService`)
- **Purpose:** Sync data from MongoDB (command DB) to SQL Server (query DB)
- **Trigger:** After each write operation

## Data Flow

### Write Flow (Command)
```
1. API receives Command (e.g., CreateOrderCommand)
2. Command Handler processes the command
3. Domain entity created/updated
4. Write to MongoDB (Command DB)
5. Project to SQL Server (Query DB) via ProjectionService
6. Return result
```

### Read Flow (Query)
```
1. API receives Query (e.g., GetOrderByIdQuery)
2. Query Handler processes the query
3. Read from SQL Server (Query DB) using ReadRepository
4. Return optimized read model (DTO)
```

## Implementation Details

### Order Service

**Command Repository:**
- `OrderMongoRepository` - Writes to MongoDB
- Database: `OrderServiceDb` (MongoDB)

**Query Repository:**
- `OrderReadRepository` - Reads from SQL Server
- Database: `OrderServiceReadDb` (SQL Server)

**Read Models:**
- `OrderReadModel` - Optimized for queries
- `OrderItemReadModel` - Denormalized order items

**Projection:**
- `OrderProjectionService` - Syncs MongoDB → SQL Server

### Product Service

**Command Repository:**
- `ProductMongoRepository` - Writes to MongoDB
- Database: `ProductServiceDb` (MongoDB)

**Query Repository:**
- `ProductReadRepository` - Reads from SQL Server
- Database: `ProductServiceReadDb` (SQL Server)

**Read Models:**
- `ProductReadModel` - Optimized for queries

**Projection:**
- `ProductProjectionService` - Syncs MongoDB → SQL Server

### User Service

**Command Repository:**
- `UserMongoRepository` - Writes to MongoDB
- Database: `UserServiceDb` (MongoDB)

**Query Repository:**
- `UserReadRepository` - Reads from SQL Server
- Database: `UserServiceReadDb` (SQL Server)

**Read Models:**
- `UserReadModel` - Optimized for queries

**Projection:**
- `UserProjectionService` - Syncs MongoDB → SQL Server

## Connection Strings

### Order Service
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "SqlServer": "Server=localhost,1433;Database=OrderServiceReadDb;..."
  }
}
```

### Product Service
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "SqlServer": "Server=localhost,1433;Database=ProductServiceReadDb;..."
  }
}
```

### User Service
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "SqlServer": "Server=localhost,1433;Database=UserServiceReadDb;..."
  }
}
```

## Benefits of This CQRS Implementation

1. **Performance:**
   - MongoDB optimized for writes (document-based, flexible schema)
   - SQL Server optimized for reads (relational, indexed queries)

2. **Scalability:**
   - Write and read databases can scale independently
   - Read replicas can be added to SQL Server

3. **Flexibility:**
   - Read models can be denormalized for optimal query performance
   - Command models can evolve without affecting queries

4. **Separation of Concerns:**
   - Clear separation between write and read operations
   - Different optimization strategies for each side

## Projection Strategy

### Current Implementation
- **Synchronous Projection:** Data is projected immediately after write
- **Location:** In Command Handlers after MongoDB write

### Future Enhancements
- **Asynchronous Projection:** Use message queue for eventual consistency
- **Event-Driven Projection:** Project on domain events
- **Batch Projection:** Project multiple changes in batches

## Database Setup

### MongoDB (Command DB)
- One database per service
- Collections: `orders`, `products`, `users`
- No schema enforcement (flexible documents)

### SQL Server (Query DB)
- One database per service (read models)
- Tables: `Orders`, `OrderItems`, `Products`, `Users`
- Indexed for query performance
- Denormalized for optimal reads

## Example Usage

### Creating an Order (Command)
```csharp
// Command Handler
var order = new Order(customerId, items);

// Write to MongoDB
await _orderRepository.AddAsync(order);

// Project to SQL Server
await _projectionService.ProjectOrderAsync(order.Id);
```

### Reading an Order (Query)
```csharp
// Query Handler
var order = await _readRepository.GetByIdAsync(orderId);
// Reads from SQL Server (optimized read model)
```

## Monitoring & Maintenance

### MongoDB
- Monitor write performance
- Check document sizes
- Index frequently queried fields

### SQL Server
- Monitor query performance
- Maintain indexes on read models
- Check projection lag

### Projection Service
- Monitor projection success/failure
- Implement retry logic for failed projections
- Log projection metrics

## Best Practices

1. **Always project after writes** - Ensure read models are up-to-date
2. **Handle projection failures** - Implement retry mechanisms
3. **Monitor consistency** - Check for projection lag
4. **Optimize read models** - Denormalize for query performance
5. **Index appropriately** - Add indexes on frequently queried fields

## Troubleshooting

### Read model not updated
- Check projection service logs
- Verify MongoDB write succeeded
- Check SQL Server connection

### Performance issues
- Review MongoDB write performance
- Check SQL Server query plans
- Optimize read model indexes

### Data inconsistency
- Implement projection retry mechanism
- Add monitoring for projection lag
- Consider eventual consistency patterns

