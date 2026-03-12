using Microsoft.EntityFrameworkCore;
using ProductODataApi.Models;

namespace ProductODataApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasMaxLength(100);
        });

        // Bulk seed data: 50 products across 8 categories
        modelBuilder.Entity<Product>().HasData(
            // Electronics
            new Product { Id = 1, Name = "Laptop Pro 15\"", Price = 1299.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "Wireless Mouse", Price = 29.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, Name = "Mechanical Keyboard", Price = 89.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, Name = "4K Monitor 27\"", Price = 449.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 5, Name = "USB-C Hub", Price = 49.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 6, Name = "Webcam HD 1080p", Price = 79.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 7, Name = "Bluetooth Speaker", Price = 59.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 8, Name = "Noise Cancelling Headphones", Price = 199.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 9, Name = "Portable SSD 1TB", Price = 109.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 10, Name = "Wireless Charger", Price = 34.99m, Category = "Electronics", CreatedDate = new DateTime(2025, 3, 5, 0, 0, 0, DateTimeKind.Utc) },

            // Furniture
            new Product { Id = 11, Name = "Ergonomic Office Chair", Price = 399.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 12, Name = "Standing Desk", Price = 549.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 1, 22, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 13, Name = "Bookshelf Oak", Price = 189.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 2, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 14, Name = "Monitor Arm", Price = 79.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 2, 18, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 15, Name = "Filing Cabinet", Price = 149.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 16, Name = "Desk Lamp LED", Price = 44.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 3, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 17, Name = "Footrest Ergonomic", Price = 59.99m, Category = "Furniture", CreatedDate = new DateTime(2025, 3, 20, 0, 0, 0, DateTimeKind.Utc) },

            // Clothing
            new Product { Id = 18, Name = "Cotton T-Shirt", Price = 19.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 19, Name = "Denim Jeans", Price = 49.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 1, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 20, Name = "Winter Jacket", Price = 129.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 2, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 21, Name = "Running Shoes", Price = 89.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 2, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 22, Name = "Wool Sweater", Price = 69.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 23, Name = "Baseball Cap", Price = 24.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 3, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 24, Name = "Leather Belt", Price = 34.99m, Category = "Clothing", CreatedDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc) },

            // Books
            new Product { Id = 25, Name = "Clean Code", Price = 39.99m, Category = "Books", CreatedDate = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 26, Name = "Design Patterns", Price = 44.99m, Category = "Books", CreatedDate = new DateTime(2025, 1, 18, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 27, Name = "The Pragmatic Programmer", Price = 42.99m, Category = "Books", CreatedDate = new DateTime(2025, 2, 6, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 28, Name = "Domain-Driven Design", Price = 54.99m, Category = "Books", CreatedDate = new DateTime(2025, 2, 22, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 29, Name = "Refactoring", Price = 47.99m, Category = "Books", CreatedDate = new DateTime(2025, 3, 4, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 30, Name = "System Design Interview", Price = 35.99m, Category = "Books", CreatedDate = new DateTime(2025, 3, 18, 0, 0, 0, DateTimeKind.Utc) },

            // Sports
            new Product { Id = 31, Name = "Yoga Mat", Price = 29.99m, Category = "Sports", CreatedDate = new DateTime(2025, 1, 7, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 32, Name = "Dumbbell Set 20kg", Price = 79.99m, Category = "Sports", CreatedDate = new DateTime(2025, 1, 28, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 33, Name = "Resistance Bands", Price = 19.99m, Category = "Sports", CreatedDate = new DateTime(2025, 2, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 34, Name = "Jump Rope", Price = 14.99m, Category = "Sports", CreatedDate = new DateTime(2025, 2, 26, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 35, Name = "Water Bottle 1L", Price = 12.99m, Category = "Sports", CreatedDate = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc) },

            // Kitchen
            new Product { Id = 36, Name = "Coffee Maker", Price = 89.99m, Category = "Kitchen", CreatedDate = new DateTime(2025, 1, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 37, Name = "Blender Pro", Price = 69.99m, Category = "Kitchen", CreatedDate = new DateTime(2025, 2, 3, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 38, Name = "Stainless Steel Cookware Set", Price = 199.99m, Category = "Kitchen", CreatedDate = new DateTime(2025, 2, 16, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 39, Name = "Air Fryer", Price = 119.99m, Category = "Kitchen", CreatedDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 40, Name = "Electric Kettle", Price = 39.99m, Category = "Kitchen", CreatedDate = new DateTime(2025, 3, 14, 0, 0, 0, DateTimeKind.Utc) },

            // Stationery
            new Product { Id = 41, Name = "Notebook A5 Pack", Price = 9.99m, Category = "Stationery", CreatedDate = new DateTime(2025, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 42, Name = "Gel Pen Set (12 pack)", Price = 14.99m, Category = "Stationery", CreatedDate = new DateTime(2025, 1, 19, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 43, Name = "Desk Organizer", Price = 24.99m, Category = "Stationery", CreatedDate = new DateTime(2025, 2, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 44, Name = "Whiteboard 60x90cm", Price = 49.99m, Category = "Stationery", CreatedDate = new DateTime(2025, 2, 24, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 45, Name = "Sticky Notes Mega Pack", Price = 7.99m, Category = "Stationery", CreatedDate = new DateTime(2025, 3, 6, 0, 0, 0, DateTimeKind.Utc) },

            // Gaming
            new Product { Id = 46, Name = "Gaming Mouse RGB", Price = 59.99m, Category = "Gaming", CreatedDate = new DateTime(2025, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 47, Name = "Gaming Headset 7.1", Price = 99.99m, Category = "Gaming", CreatedDate = new DateTime(2025, 1, 26, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 48, Name = "Mousepad XL", Price = 24.99m, Category = "Gaming", CreatedDate = new DateTime(2025, 2, 9, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 49, Name = "Controller Wireless", Price = 54.99m, Category = "Gaming", CreatedDate = new DateTime(2025, 2, 23, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 50, Name = "Gaming Chair", Price = 299.99m, Category = "Gaming", CreatedDate = new DateTime(2025, 3, 7, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
