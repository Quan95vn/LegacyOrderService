using LegacyOrderService.Data;
using LegacyOrderService.Extensions;
using LegacyOrderService.Interfaces;
using LegacyOrderService.Services;
using LegacyOrderService.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LegacyOrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<OrderService>();

            var serviceProvider = services.BuildServiceProvider();

            await SeedDataExtensions.SeedDataAsync(serviceProvider);

            var orderService = serviceProvider.GetRequiredService<OrderService>();
            var appLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            appLogger.LogInformation("Application Starting...");
            Console.WriteLine("Welcome to Order Processor!");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true; 
                cts.Cancel();

                ConsoleHelper.WriteWarning("\nShutdown requested. Waiting for pending tasks...");

                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(3000);
                    }
                    finally
                    {
                        ConsoleHelper.WriteError("Force exiting application.");
                        Environment.Exit(0);
                    }
                });
            };

            while (true)
            {
                try
                {
                    string name = ConsoleHelper.GetValidStringInput("Enter customer name: ");
                    string product = ConsoleHelper.GetValidStringInput("Enter product name: ");
                    long qty = ConsoleHelper.GetValidLongInput("Enter quantity: ");

                    Console.WriteLine("Processing order...");
                    Console.WriteLine("Saving order to database...");
                    var result = await orderService.ProcessOrderAsync(name, product, qty, cts.Token);

                    if (result.IsSuccess)
                    {
                        var order = result.Value;
                        ConsoleHelper.WriteSuccess("Order complete!");
                        ConsoleHelper.WriteSuccess("Customer: " + order.CustomerName);
                        ConsoleHelper.WriteSuccess("Product: " + order.Product.Name);
                        ConsoleHelper.WriteSuccess("Quantity: " + order.Quantity);
                        ConsoleHelper.WriteSuccess("Total: $" + order.Quantity * order.Price);
                    }
                    else
                    {
                        ConsoleHelper.WriteWarning($"Order Failed: {result.ErrorMessage}");
                    }

                    Console.Write("Do you want to process another order? (y/n): ");

                    string? choice = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(choice) || !choice.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Exiting application. Goodbye!");
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    ConsoleHelper.WriteError("Operation canceled by user.");
                    break;
                }
                catch (Exception)
                {
                    ConsoleHelper.WriteError("Something went wrong. Please check logs.");
                }
            }

            Log.CloseAndFlush();
        }
    }
}
