using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using NewCoreProject.Models;

namespace NewCoreProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public virtual DbSet<User> Admins { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order_Detail> Order_Detail { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public DbSet<NewCoreProject.Models.User> User { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category → Products (one-to-many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.Category_Fid)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<Order_Detail>()
                .Property(o => o.Sale_Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Order_Detail>()
                .Property(o => o.Purchase_Price)
                .HasPrecision(18, 0);

            // Order → Order_Detail (one-to-many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Order_Detail)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.Order_Fid)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
