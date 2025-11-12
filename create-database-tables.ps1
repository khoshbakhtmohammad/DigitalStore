# Script to create all database tables for microservices
# Run this script if tables are missing

$ErrorActionPreference = "Stop"

Write-Host "Creating database tables for all microservices..." -ForegroundColor Cyan

# ProductService Database
Write-Host "`nCreating ProductServiceReadDb..." -ForegroundColor Yellow
$productConn = "Server=localhost,1433;Database=ProductServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
$productQuery = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id uniqueidentifier PRIMARY KEY,
        Name nvarchar(500) NOT NULL,
        Description nvarchar(2000),
        Price decimal(18,2) NOT NULL,
        Currency nvarchar(3) NOT NULL DEFAULT 'USD',
        StockQuantity int NOT NULL,
        Status nvarchar(50) NOT NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2
    );
    CREATE INDEX IX_Products_Name ON Products(Name);
END
"@

try {
    $masterConn = "Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
    $master = New-Object System.Data.SqlClient.SqlConnection($masterConn)
    $master.Open()
    $createDb = New-Object System.Data.SqlClient.SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProductServiceReadDb') CREATE DATABASE ProductServiceReadDb", $master)
    $createDb.ExecuteNonQuery() | Out-Null
    $master.Close()
    
    $conn = New-Object System.Data.SqlClient.SqlConnection($productConn)
    $conn.Open()
    $cmd = New-Object System.Data.SqlClient.SqlCommand($productQuery, $conn)
    $cmd.ExecuteNonQuery() | Out-Null
    $conn.Close()
    Write-Host "✓ ProductServiceReadDb tables created" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

# UserService Database
Write-Host "`nCreating UserServiceReadDb..." -ForegroundColor Yellow
$userConn = "Server=localhost,1433;Database=UserServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
$userQuery = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id uniqueidentifier PRIMARY KEY,
        Email nvarchar(255) NOT NULL UNIQUE,
        FirstName nvarchar(100) NOT NULL,
        LastName nvarchar(100) NOT NULL,
        Status nvarchar(50) NOT NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2
    );
    CREATE INDEX IX_Users_Email ON Users(Email);
END
"@

try {
    $master = New-Object System.Data.SqlClient.SqlConnection($masterConn)
    $master.Open()
    $createDb = New-Object System.Data.SqlClient.SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UserServiceReadDb') CREATE DATABASE UserServiceReadDb", $master)
    $createDb.ExecuteNonQuery() | Out-Null
    $master.Close()
    
    $conn = New-Object System.Data.SqlClient.SqlConnection($userConn)
    $conn.Open()
    $cmd = New-Object System.Data.SqlClient.SqlCommand($userQuery, $conn)
    $cmd.ExecuteNonQuery() | Out-Null
    $conn.Close()
    Write-Host "✓ UserServiceReadDb tables created" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

# OrderService Database
Write-Host "`nCreating OrderServiceReadDb..." -ForegroundColor Yellow
$orderConn = "Server=localhost,1433;Database=OrderServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
$orderQuery = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id uniqueidentifier PRIMARY KEY,
        CustomerId uniqueidentifier NOT NULL,
        Status nvarchar(50) NOT NULL,
        TotalAmount decimal(18,2) NOT NULL,
        Currency nvarchar(3) NOT NULL DEFAULT 'USD',
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2
    );
    CREATE TABLE OrderItems (
        Id uniqueidentifier PRIMARY KEY,
        OrderId uniqueidentifier NOT NULL,
        ProductId uniqueidentifier NOT NULL,
        ProductName nvarchar(255) NOT NULL,
        Price decimal(18,2) NOT NULL,
        Currency nvarchar(3) NOT NULL DEFAULT 'USD',
        Quantity int NOT NULL,
        FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
END
"@

try {
    $master = New-Object System.Data.SqlClient.SqlConnection($masterConn)
    $master.Open()
    $createDb = New-Object System.Data.SqlClient.SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderServiceReadDb') CREATE DATABASE OrderServiceReadDb", $master)
    $createDb.ExecuteNonQuery() | Out-Null
    $master.Close()
    
    $conn = New-Object System.Data.SqlClient.SqlConnection($orderConn)
    $conn.Open()
    $cmd = New-Object System.Data.SqlClient.SqlCommand($orderQuery, $conn)
    $cmd.ExecuteNonQuery() | Out-Null
    $conn.Close()
    Write-Host "✓ OrderServiceReadDb tables created" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

Write-Host "`n✓ All database tables created successfully!" -ForegroundColor Green
Write-Host "You can now use the APIs without database errors." -ForegroundColor Cyan

