# Database Schema Documentation

## Overview
This microservices architecture uses CQRS pattern with:
- **MongoDB** for Command/Write operations
- **SQL Server** for Query/Read operations

## MongoDB Schemas (Command Database)

### OrderServiceDb
- **Collection: Orders**
  - `_id` (Guid)
  - `Status` (string)
  - `CustomerId` (Guid)
  - `TotalAmount` (decimal)
  - `CreatedAt` (DateTime)
  - `UpdatedAt` (DateTime?)
  - `Items` (array of OrderItem)
    - `_id` (Guid)
    - `ProductId` (Guid)
    - `ProductName` (string)
    - `Price` (decimal)
    - `Quantity` (int)

### ProductServiceDb
- **Collection: Products**
  - `_id` (Guid)
  - `Name` (string)
  - `Description` (string)
  - `Price` (decimal)
  - `StockQuantity` (int)
  - `Status` (string)
  - `CreatedAt` (DateTime)
  - `UpdatedAt` (DateTime?)

### UserServiceDb
- **Collection: Users**
  - `_id` (Guid)
  - `Email` (string)
  - `FirstName` (string)
  - `LastName` (string)
  - `Status` (string)
  - `CreatedAt` (DateTime)
  - `UpdatedAt` (DateTime?)

## SQL Server Schemas (Query Database)

### OrderService Read Database
- **Table: Orders**
  - `Id` (uniqueidentifier, PK)
  - `CustomerId` (uniqueidentifier)
  - `Status` (nvarchar(50))
  - `TotalAmount` (decimal(18,2))
  - `Currency` (nvarchar(3))
  - `CreatedAt` (datetime2)
  - `UpdatedAt` (datetime2, nullable)

- **Table: OrderItems**
  - `Id` (uniqueidentifier, PK)
  - `OrderId` (uniqueidentifier, FK -> Orders.Id)
  - `ProductId` (uniqueidentifier)
  - `ProductName` (nvarchar(255))
  - `Price` (decimal(18,2))
  - `Currency` (nvarchar(3))
  - `Quantity` (int)

### ProductService Read Database
- **Table: Products**
  - `Id` (uniqueidentifier, PK)
  - `Name` (nvarchar(255))
  - `Description` (nvarchar(max))
  - `Price` (decimal(18,2))
  - `Currency` (nvarchar(3))
  - `StockQuantity` (int)
  - `Status` (nvarchar(50))
  - `CreatedAt` (datetime2)
  - `UpdatedAt` (datetime2, nullable)

### UserService Read Database
- **Table: Users**
  - `Id` (uniqueidentifier, PK)
  - `Email` (nvarchar(255), unique)
  - `FirstName` (nvarchar(100))
  - `LastName` (nvarchar(100))
  - `Status` (nvarchar(50))
  - `CreatedAt` (datetime2)
  - `UpdatedAt` (datetime2, nullable)

## OpenSleigh Saga State Database
- **Table: SagaStates**
  - `Id` (nvarchar(450), PK)
  - `InstanceId` (uniqueidentifier)
  - `CorrelationId` (uniqueidentifier)
  - `State` (nvarchar(max))
  - `Status` (nvarchar(50))
  - `CreatedAt` (datetime2)
  - `UpdatedAt` (datetime2)

- **Table: OutboxMessages**
  - `Id` (uniqueidentifier, PK)
  - `CorrelationId` (uniqueidentifier)
  - `MessageType` (nvarchar(255))
  - `Body` (nvarchar(max))
  - `Status` (nvarchar(50))
  - `CreatedAt` (datetime2)
  - `ProcessedAt` (datetime2, nullable)

## Redis Cache Schema
- **Key Pattern: `idempotency:{idempotencyKey}`**
  - Value: OrderId (Guid as string)
  - TTL: 24 hours

## Connection Strings

### MongoDB
```
mongodb://localhost:27017
```

### SQL Server
```
Server=localhost,1433;Database={ServiceName}ReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

### Redis
```
localhost:6379
```

### RabbitMQ
```
HostName=localhost;Port=5672;UserName=guest;Password=guest;VHost=/
```

