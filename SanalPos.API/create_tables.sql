USE SanalPosDb2;

-- ProductCategories tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductCategories')
BEGIN
    CREATE TABLE ProductCategories (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Description nvarchar(500) NOT NULL,
        ImageUrl nvarchar(255) NULL,
        IsActive bit NOT NULL,
        DisplayOrder int NOT NULL,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_ProductCategories PRIMARY KEY (Id)
    )
    PRINT 'ProductCategories tablosu oluşturuldu.'
END
ELSE
    PRINT 'ProductCategories tablosu zaten mevcut.'

-- Products tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Description nvarchar(500) NOT NULL,
        Price decimal(18,2) NOT NULL,
        StockQuantity int NOT NULL,
        ImageUrl nvarchar(255) NOT NULL,
        ProductCategoryId uniqueidentifier NOT NULL,
        IsActive bit NOT NULL,
        IsAvailable bit NOT NULL,
        PreparationTimeMinutes int NOT NULL DEFAULT 0,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_Products PRIMARY KEY (Id),
        CONSTRAINT FK_Products_ProductCategories_ProductCategoryId FOREIGN KEY (ProductCategoryId) REFERENCES ProductCategories (Id) ON DELETE CASCADE
    )
    PRINT 'Products tablosu oluşturuldu.'
END
ELSE
    PRINT 'Products tablosu zaten mevcut.'

-- Orders tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id uniqueidentifier NOT NULL,
        OrderNumber nvarchar(20) NOT NULL,
        OrderDate datetime2 NOT NULL,
        TotalAmount decimal(18,2) NOT NULL,
        Status int NOT NULL,
        PaymentMethod int NOT NULL,
        PaymentStatus int NOT NULL,
        CustomerName nvarchar(100) NOT NULL,
        CustomerEmail nvarchar(100) NOT NULL,
        CustomerPhone nvarchar(20) NOT NULL,
        ShippingAddress nvarchar(500) NOT NULL,
        BillingAddress nvarchar(500) NOT NULL,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_Orders PRIMARY KEY (Id)
    )
    PRINT 'Orders tablosu oluşturuldu.'
END
ELSE
    PRINT 'Orders tablosu zaten mevcut.'

-- OrderItems tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id uniqueidentifier NOT NULL,
        OrderId uniqueidentifier NOT NULL,
        ProductId uniqueidentifier NOT NULL,
        Quantity int NOT NULL,
        UnitPrice decimal(18,2) NOT NULL,
        TotalPrice decimal(18,2) NOT NULL,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_OrderItems PRIMARY KEY (Id),
        CONSTRAINT FK_OrderItems_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products_ProductId FOREIGN KEY (ProductId) REFERENCES Products (Id) ON DELETE CASCADE
    )
    PRINT 'OrderItems tablosu oluşturuldu.'
END
ELSE
    PRINT 'OrderItems tablosu zaten mevcut.'

-- Payments tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
BEGIN
    CREATE TABLE Payments (
        Id uniqueidentifier NOT NULL,
        OrderId uniqueidentifier NOT NULL,
        Amount decimal(18,2) NOT NULL,
        PaymentDate datetime2 NOT NULL,
        PaymentMethod int NOT NULL,
        TransactionId nvarchar(100) NOT NULL,
        Status int NOT NULL,
        ErrorMessage nvarchar(500) NULL,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_Payments PRIMARY KEY (Id),
        CONSTRAINT FK_Payments_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE
    )
    PRINT 'Payments tablosu oluşturuldu.'
END
ELSE
    PRINT 'Payments tablosu zaten mevcut.'

-- PaymentProviders tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PaymentProviders')
BEGIN
    CREATE TABLE PaymentProviders (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Description nvarchar(500) NOT NULL,
        IsActive bit NOT NULL,
        ApiKey nvarchar(100) NOT NULL,
        ApiSecret nvarchar(100) NOT NULL,
        BaseUrl nvarchar(255) NOT NULL,
        ProviderType int NOT NULL,
        CreatedAt datetime2 NOT NULL,
        CreatedBy nvarchar(100) NOT NULL,
        LastModifiedAt datetime2 NULL,
        LastModifiedBy nvarchar(100) NOT NULL,
        IsDeleted bit NOT NULL,
        CONSTRAINT PK_PaymentProviders PRIMARY KEY (Id)
    )
    PRINT 'PaymentProviders tablosu oluşturuldu.'
END
ELSE
    PRINT 'PaymentProviders tablosu zaten mevcut.'

PRINT 'Tüm tablolar başarıyla oluşturuldu veya zaten mevcuttu.' 