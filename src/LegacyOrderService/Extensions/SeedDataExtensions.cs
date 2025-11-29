using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyOrderService.Extensions;

public static class SeedDataExtensions
{
    public static void Seed(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<OrderDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                context.Database.Migrate();

                if (context.Products.Any())
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
                context.SaveChanges();
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
