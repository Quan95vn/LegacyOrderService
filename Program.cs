using LegacyOrderService.Models;
using LegacyOrderService.Data;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Order Processor!");

            while (true)
            {
                string name = GetValidStringInput("Enter customer name: ");
                string product = GetValidStringInput("Enter product name: ");

                var productRepo = new ProductRepository();
                double price = productRepo.GetPrice(product);

                int qty = GetValidIntInput("Enter quantity: ");

                Console.WriteLine("Processing order...");

                Order order = new Order();
                order.CustomerName = name;
                order.ProductName = product;
                order.Quantity = qty;
                order.Price = price;

                double total = order.Quantity * order.Price;

                var repo = new OrderRepository();
                repo.Save(order);
                Console.WriteLine("Saving order to database...");

                Console.WriteLine("Order complete!");
                Console.WriteLine("Customer: " + order.CustomerName);
                Console.WriteLine("Product: " + order.ProductName);
                Console.WriteLine("Quantity: " + order.Quantity);
                Console.WriteLine("Total: $" + total);

                Console.WriteLine("Order processed successfully.");
                Console.Write("Do you want to process another order? (y/n): ");
                string? choice = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(choice) || !choice.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting application. Goodbye!");
                    break;
                }
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
        }
    }
}
