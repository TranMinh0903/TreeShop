-- ============================================
-- AUTO-INIT SCRIPT FOR DOCKER SQL SERVER
-- Chá»‰ táº¡o DB náº¿u chÆ°a tá»“n táº¡i
-- ============================================

IF DB_ID('ShopDB') IS NULL
BEGIN
    CREATE DATABASE ShopDB;
END
GO

USE ShopDB;
GO

-- ============================================
-- 1. Account Table
-- ============================================

IF OBJECT_ID('dbo.Account', 'U') IS NULL
BEGIN
    CREATE TABLE Account (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        Username       NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash   NVARCHAR(255) NOT NULL,
        Email          NVARCHAR(150) NOT NULL UNIQUE,
        Role           NVARCHAR(50) DEFAULT 'User',
        CreatedAt      DATETIME DEFAULT GETDATE()
    );
END
GO

-- ============================================
-- 2. Category Table
-- ============================================

IF OBJECT_ID('dbo.Category', 'U') IS NULL
BEGIN
    CREATE TABLE Category (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName   NVARCHAR(150) NOT NULL,
        Description    NVARCHAR(255),
        [Status]       BIT,
        CreatedAt      DATETIME DEFAULT GETDATE()
    );
END
GO

-- ============================================
-- 3. Product Table
-- ============================================

IF OBJECT_ID('dbo.Product', 'U') IS NULL
BEGIN
    CREATE TABLE Product (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        ProductName    NVARCHAR(200) NOT NULL,
        Price          DECIMAL(18,2) NOT NULL,
        StockQuantity  INT DEFAULT 0,
        [Status]       BIT,
        CreatedBy      INT NULL,
        CreatedAt      DATETIME DEFAULT GETDATE(),
        ImageUrl       NVARCHAR(500) NULL,
        CategoryId     INT NOT NULL,
        CONSTRAINT FK_Category_Product
            FOREIGN KEY (CategoryId)
            REFERENCES Category(Id)
    );
END
GO

-- ============================================
-- 4. Order Table
-- ============================================

IF OBJECT_ID('dbo.[Order]', 'U') IS NULL
BEGIN
    CREATE TABLE [Order] (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        UserId              INT NOT NULL,
        ShipperId           INT NULL,
        ReceiverName        NVARCHAR(200) NOT NULL,
        ReceiverPhone       NVARCHAR(20) NOT NULL,
        ShippingAddress     NVARCHAR(500) NOT NULL,
        ShippingMethod      NVARCHAR(50) DEFAULT 'Standard',
        ShippingFee         DECIMAL(18,2) DEFAULT 0,
        TotalPrice          DECIMAL(18,2) NOT NULL,
        Status              NVARCHAR(50) DEFAULT 'Pending',
        PaymentMethod       NVARCHAR(50) DEFAULT 'COD',
        DeliveryImageUrl    NVARCHAR(500) NULL,
        DeliveryTimestamp   DATETIME NULL,
        Note                NVARCHAR(500) NULL,
        CreatedAt           DATETIME DEFAULT GETDATE(),
        UpdatedAt           DATETIME NULL,

        CONSTRAINT FK_Order_User FOREIGN KEY (UserId) REFERENCES Account(Id),
        CONSTRAINT FK_Order_Shipper FOREIGN KEY (ShipperId) REFERENCES Account(Id)
    );
END
GO

-- ============================================
-- 5. OrderDetail Table
-- ============================================

IF OBJECT_ID('dbo.OrderDetail', 'U') IS NULL
BEGIN
    CREATE TABLE OrderDetail (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        OrderId     INT NOT NULL,
        ProductId   INT NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Price       DECIMAL(18,2) NOT NULL,
        Quantity    INT NOT NULL,
        ImageUrl    NVARCHAR(500) NULL,

        CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderId) REFERENCES [Order](Id),
        CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
    );
END
GO

PRINT '=== ShopDB initialized successfully! ===';
GO
