using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<OrderDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("Data Source="))
                {
                    var pathPart = connectionString.Split("Data Source=")[1].Split(';')[0].Trim();

                    var dbFilePath = Path.GetFullPath(pathPart);
                    var dbDirectory = Path.GetDirectoryName(dbFilePath);

                    if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
                    {
                        Directory.CreateDirectory(dbDirectory);
                        logger.LogInformation("Created missing database directory: {Directory}", dbDirectory);
                    }
                }

                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database initialized and seeded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the DB: {ex.Message}");
                throw;
            }
        }
    }
}
