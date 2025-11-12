# Docker Setup Guide

This guide explains how to run all microservices using Docker Compose.

## Prerequisites

- Docker Desktop installed and running
- Docker Compose v3.8 or higher

## Services

The docker-compose.yml includes the following services:

### Infrastructure Services
- **sqlserver**: SQL Server 2022 (port 1433)
- **mongodb**: MongoDB 7 (port 27017)
- **rabbitmq**: RabbitMQ with Management UI (ports 5672, 15672)
- **redis**: Redis 7 (port 6379)

### Application Services
- **orderservice-api**: Order Service API (port 5000)
- **userservice-api**: User Service API (port 5003)
- **productservice-api**: Product Service API (port 5002)
- **gateway-api**: API Gateway (port 5001)
- **orchestrator**: Order Orchestrator Service (no exposed ports)

## Running the Services

### Start All Services

```bash
docker-compose up -d
```

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f orderservice-api
```

### Stop All Services

```bash
docker-compose down
```

### Stop and Remove Volumes

```bash
docker-compose down -v
```

### Rebuild Services

```bash
docker-compose build --no-cache
docker-compose up -d
```

## Accessing Services

- **API Gateway**: http://localhost:5001/swagger
- **Order Service**: http://localhost:5000/swagger
- **User Service**: http://localhost:5003/swagger
- **Product Service**: http://localhost:5002/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **SQL Server**: localhost:1433 (sa/YourStrong@Passw0rd)

## Service Dependencies

Services start in the following order:
1. Infrastructure services (SQL Server, MongoDB, RabbitMQ, Redis)
2. Microservices (Order, User, Product)
3. Gateway (depends on all microservices)
4. Orchestrator (depends on SQL Server and RabbitMQ)

## Environment Variables

Connection strings are configured via environment variables in docker-compose.yml:
- Services use Docker service names (e.g., `sqlserver`, `mongodb`) instead of `localhost`
- All connection strings are set via environment variables

## Troubleshooting

### Check Service Status

```bash
docker-compose ps
```

### Check Service Health

```bash
docker-compose ps --format "table {{.Name}}\t{{.Status}}"
```

### Restart a Specific Service

```bash
docker-compose restart orderservice-api
```

### View Service Logs

```bash
docker-compose logs orderservice-api
```

### Access Container Shell

```bash
docker exec -it orderservice-api /bin/bash
```

## Database Initialization

Databases are automatically created when services start. The services will:
- Create SQL Server databases if they don't exist
- Run migrations automatically
- Initialize MongoDB collections

## Notes

- First startup may take several minutes as Docker builds images
- Services wait for infrastructure services to be healthy before starting
- All data is persisted in Docker volumes

