using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<OrderDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                await context.Database.EnsureCreatedAsync();

                if (await context.Products.AnyAsync())
                {
                    return;
                }

                logger.LogInformation("Seeding Product data...");
                var products = new Product[]
                {
                    new() { Name = "Widget", Price = 12.99 },
                    new() { Name = "Gadget", Price = 15.49 },
                    new() { Name = "Doohickey", Price = 8.75 }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeding done.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"An error occurred seeding the DB: {ex.Message}");
            }
        }
    }
}
