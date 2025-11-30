using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

           
            entity.HasOne(o => o.Product)    
                  .WithMany()                  
                  .HasForeignKey(o => o.ProductId) 
                  .IsRequired()               
                  .OnDelete(DeleteBehavior.Restrict); 

            entity.Property(o => o.CustomerName).IsRequired();
            entity.Property(o => o.Price).HasColumnType("REAL"); 
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired();
        });
    }
}
