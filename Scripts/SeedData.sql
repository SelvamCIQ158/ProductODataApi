-- ============================================
-- ProductODataApi - Bulk Seed Data Script
-- Database: ProductODataDb
-- ============================================

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ProductODataDb')
BEGIN
    CREATE DATABASE ProductODataDb;
END
GO

USE ProductODataDb;
GO

-- Create Products table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(200) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Clear existing data for clean seed
DELETE FROM Products;
DBCC CHECKIDENT ('Products', RESEED, 0);
GO

-- ============================================
-- BULK INSERT: 50 Products across 8 categories
-- ============================================

INSERT INTO Products (Name, Price, Category, CreatedDate) VALUES
-- Electronics (10 items)
('Laptop Pro 15"', 1299.99, 'Electronics', '2025-01-05'),
('Wireless Mouse', 29.99, 'Electronics', '2025-01-10'),
('Mechanical Keyboard', 89.99, 'Electronics', '2025-01-15'),
('4K Monitor 27"', 449.99, 'Electronics', '2025-01-20'),
('USB-C Hub', 49.99, 'Electronics', '2025-02-01'),
('Webcam HD 1080p', 79.99, 'Electronics', '2025-02-10'),
('Bluetooth Speaker', 59.99, 'Electronics', '2025-02-15'),
('Noise Cancelling Headphones', 199.99, 'Electronics', '2025-02-20'),
('Portable SSD 1TB', 109.99, 'Electronics', '2025-03-01'),
('Wireless Charger', 34.99, 'Electronics', '2025-03-05'),

-- Furniture (7 items)
('Ergonomic Office Chair', 399.99, 'Furniture', '2025-01-08'),
('Standing Desk', 549.99, 'Furniture', '2025-01-22'),
('Bookshelf Oak', 189.99, 'Furniture', '2025-02-05'),
('Monitor Arm', 79.99, 'Furniture', '2025-02-18'),
('Filing Cabinet', 149.99, 'Furniture', '2025-03-02'),
('Desk Lamp LED', 44.99, 'Furniture', '2025-03-12'),
('Footrest Ergonomic', 59.99, 'Furniture', '2025-03-20'),

-- Clothing (7 items)
('Cotton T-Shirt', 19.99, 'Clothing', '2025-01-12'),
('Denim Jeans', 49.99, 'Clothing', '2025-01-25'),
('Winter Jacket', 129.99, 'Clothing', '2025-02-02'),
('Running Shoes', 89.99, 'Clothing', '2025-02-14'),
('Wool Sweater', 69.99, 'Clothing', '2025-02-28'),
('Baseball Cap', 24.99, 'Clothing', '2025-03-08'),
('Leather Belt', 34.99, 'Clothing', '2025-03-15'),

-- Books (6 items)
('Clean Code', 39.99, 'Books', '2025-01-03'),
('Design Patterns', 44.99, 'Books', '2025-01-18'),
('The Pragmatic Programmer', 42.99, 'Books', '2025-02-06'),
('Domain-Driven Design', 54.99, 'Books', '2025-02-22'),
('Refactoring', 47.99, 'Books', '2025-03-04'),
('System Design Interview', 35.99, 'Books', '2025-03-18'),

-- Sports (5 items)
('Yoga Mat', 29.99, 'Sports', '2025-01-07'),
('Dumbbell Set 20kg', 79.99, 'Sports', '2025-01-28'),
('Resistance Bands', 19.99, 'Sports', '2025-02-12'),
('Jump Rope', 14.99, 'Sports', '2025-02-26'),
('Water Bottle 1L', 12.99, 'Sports', '2025-03-10'),

-- Kitchen (5 items)
('Coffee Maker', 89.99, 'Kitchen', '2025-01-14'),
('Blender Pro', 69.99, 'Kitchen', '2025-02-03'),
('Stainless Steel Cookware Set', 199.99, 'Kitchen', '2025-02-16'),
('Air Fryer', 119.99, 'Kitchen', '2025-03-01'),
('Electric Kettle', 39.99, 'Kitchen', '2025-03-14'),

-- Stationery (5 items)
('Notebook A5 Pack', 9.99, 'Stationery', '2025-01-06'),
('Gel Pen Set (12 pack)', 14.99, 'Stationery', '2025-01-19'),
('Desk Organizer', 24.99, 'Stationery', '2025-02-08'),
('Whiteboard 60x90cm', 49.99, 'Stationery', '2025-02-24'),
('Sticky Notes Mega Pack', 7.99, 'Stationery', '2025-03-06'),

-- Gaming (5 items)
('Gaming Mouse RGB', 59.99, 'Gaming', '2025-01-11'),
('Gaming Headset 7.1', 99.99, 'Gaming', '2025-01-26'),
('Mousepad XL', 24.99, 'Gaming', '2025-02-09'),
('Controller Wireless', 54.99, 'Gaming', '2025-02-23'),
('Gaming Chair', 299.99, 'Gaming', '2025-03-07');

GO

-- Verify the seed data
SELECT COUNT(*) AS TotalProducts FROM Products;
SELECT Category, COUNT(*) AS Count, AVG(Price) AS AvgPrice
FROM Products
GROUP BY Category
ORDER BY Category;
GO
