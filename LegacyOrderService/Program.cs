using LegacyOrderService.Common;
using LegacyOrderService.Data;
using LegacyOrderService.Interfaces;
using LegacyOrderService.Models;
using LegacyOrderService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
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

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<OrderService>();

            var serviceProvider = services.BuildServiceProvider();


            var orderService = serviceProvider.GetRequiredService<OrderService>();
            var appLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            appLogger.LogInformation("Application Starting...");
            Console.WriteLine("Welcome to Order Processor!");

            while (true)
            {
                try
                {
                    string name = GetValidStringInput("Enter customer name: ");
                    string product = GetValidStringInput("Enter product name: ");
                    int qty = GetValidIntInput("Enter quantity: ");

                    Console.WriteLine("Processing order...");
                    Console.WriteLine("Saving order to database...");
                    var result = orderService.ProcessOrder(name, product, qty);

                    if (result.IsSuccess)
                    {
                        var order = result.Value;
                        WriteSuccess("Order complete!");
                        WriteSuccess("Customer: " + order.CustomerName);
                        WriteSuccess("Product: " + order.ProductName);
                        WriteSuccess("Quantity: " + order.Quantity);
                        WriteSuccess("Total: $" + order.Quantity * order.Price);
                    }
                    else
                    {
                        WriteWarning($"Order Failed: {result.ErrorMessage}");
                    }
                  
                    Console.Write("Do you want to process another order? (y/n): ");
                    
                    string? choice = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(choice) || !choice.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Exiting application. Goodbye!");
                        break;
                    }
                }
                catch (Exception)
                {
                    WriteError("Something went wrong. Please check logs.");
                }
            }

            Log.CloseAndFlush();
        }

        static string GetValidStringInput(string prompt)
        {
            string? input = "";
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return input;
        }

        static int GetValidIntInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                {
                    if (value > 0)
                    {
                        return value;
                    }
                    Console.WriteLine("Quantity must be greater than 0.");
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
