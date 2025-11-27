using LegacyOrderService.Common;
using LegacyOrderService.Interfaces;
using LegacyOrderService.Models;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Services;

public class OrderService
{
    private readonly IProductRepository _productRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IProductRepository productRepo, 
        IOrderRepository orderRepo,
        ILogger<OrderService> logger)
    {
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _logger = logger;
    }

    public Result<Order> ProcessOrder(string customerName, string productName, int quantity)
    {
        try
        {
            double? price = _productRepo.GetPrice(productName);

            if (price == null)
            {
                _logger.LogWarning("Product not found: {ProductName}", productName);
                return Result<Order>.Failure($"Product '{productName}' does not exist.");
            }

            var order = new Order
            {
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                Price = price.Value
            };
            _orderRepo.Save(order);

            double total = order.Quantity * order.Price;
            _logger.LogInformation("Order processed successfully for Customer: {Customer}, Product: {Product}, Quantity: {Quantity}, Total Amount: ${TotalAmount}", 
                customerName, productName, quantity, total);

            return Result<Order>.Success(order);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to process order for {CustomerName}", customerName);
            throw;
        }
    }
}
