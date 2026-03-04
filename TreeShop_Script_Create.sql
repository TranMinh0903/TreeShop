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
    ImageUrl       NVARCHAR(500) NULL,       -- 2026-03-03: Thêm URL hình ảnh sản phẩm (Cloudinary)

   CategoryId INT NOT NULL,
       CONSTRAINT FK_Category_Product
        FOREIGN KEY (CategoryId)
        REFERENCES Category(Id)
);
GO


-- ============================================
-- 4. Order Table (2026-03-04: Order System)
-- Theo yêu cầu đề bài: ID, UserID, TotalPrice, Status, ShippingAddress, shipper_id, delivery_image_url, delivery_timestamp
-- ============================================

CREATE TABLE [Order] (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    UserId              INT NOT NULL,                          -- FK → Account (Customer)
    ShipperId           INT NULL,                              -- FK → Account (Shipper, assigned by Admin)
    ReceiverName        NVARCHAR(200) NOT NULL,                -- Tên người nhận
    ReceiverPhone       NVARCHAR(20) NOT NULL,                 -- SĐT người nhận
    ShippingAddress     NVARCHAR(500) NOT NULL,                -- Địa chỉ giao hàng
    ShippingMethod      NVARCHAR(50) DEFAULT 'Standard',       -- 'Standard' / 'Express'
    ShippingFee         DECIMAL(18,2) DEFAULT 0,               -- Phí ship
    TotalPrice          DECIMAL(18,2) NOT NULL,                -- Tổng tiền (items + ship)
    Status              NVARCHAR(50) DEFAULT 'Pending',        -- Pending/Confirmed/Shipping/Delivered/Completed/Cancelled
    PaymentMethod       NVARCHAR(50) DEFAULT 'COD',            -- COD / BankTransfer
    DeliveryImageUrl    NVARCHAR(500) NULL,                    -- Ảnh chứng minh giao hàng (Proof of Delivery)
    DeliveryTimestamp   DATETIME NULL,                         -- Thời điểm giao hàng hoàn tất
    Note                NVARCHAR(500) NULL,                    -- Ghi chú của khách
    CreatedAt           DATETIME DEFAULT GETDATE(),
    UpdatedAt           DATETIME NULL,

    CONSTRAINT FK_Order_User FOREIGN KEY (UserId) REFERENCES Account(Id),
    CONSTRAINT FK_Order_Shipper FOREIGN KEY (ShipperId) REFERENCES Account(Id)
);
GO


-- ============================================
-- 5. OrderDetail Table (2026-03-04: Order System)
-- Theo yêu cầu đề bài: Id, OrderId, price, quantity
-- Thêm ProductId, ProductName, ImageUrl để snapshot thông tin sản phẩm tại thời điểm đặt
-- ============================================

CREATE TABLE OrderDetail (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL,                              -- FK → Order
    ProductId   INT NOT NULL,                              -- FK → Product
    ProductName NVARCHAR(200) NOT NULL,                    -- Snapshot tên SP (phòng khi SP bị sửa/xóa)
    Price       DECIMAL(18,2) NOT NULL,                    -- Snapshot giá tại thời điểm đặt
    Quantity    INT NOT NULL,
    ImageUrl    NVARCHAR(500) NULL,                        -- Snapshot ảnh SP

    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderId) REFERENCES [Order](Id),
    CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
);
GO

