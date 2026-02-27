-- ============================================
-- CREATE DATABASE
-- ============================================

IF DB_ID('ShopDB') IS NOT NULL
    DROP DATABASE ShopDB;
GO

CREATE DATABASE ShopDB;
GO

USE ShopDB;
GO

-- ============================================
-- DROP TABLE nếu tồn tại
-- ============================================

IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL DROP TABLE dbo.Category;
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE dbo.Product;
IF OBJECT_ID('dbo.Account', 'U') IS NOT NULL DROP TABLE dbo.Account;
GO

-- ============================================
-- 1. Account Table
-- ============================================

CREATE TABLE Account (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Username       NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash   NVARCHAR(255) NOT NULL,
    Email          NVARCHAR(150) NOT NULL UNIQUE,
    Role           NVARCHAR(50) DEFAULT 'User',
    CreatedAt      DATETIME DEFAULT GETDATE()
);
GO

-- ============================================
-- 3. Category Table (N)
-- Category có FK trỏ về Product
-- ============================================

CREATE TABLE Category (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName   NVARCHAR(150) NOT NULL,
    Description    NVARCHAR(255),
	[Status] BIT, 

    CreatedAt      DATETIME DEFAULT GETDATE(),

);
GO


-- ============================================
-- 2. Product Table (1)
-- ============================================

CREATE TABLE Product (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    ProductName    NVARCHAR(200) NOT NULL,
    Price          DECIMAL(18,2) NOT NULL,
    StockQuantity  INT DEFAULT 0,
	[Status] BIT, 
    CreatedBy      INT NULL,
    CreatedAt      DATETIME DEFAULT GETDATE(),

   CategoryId INT NOT NULL,
       CONSTRAINT FK_Category_Product
        FOREIGN KEY (CategoryId)
        REFERENCES Category(Id)
);
GO

