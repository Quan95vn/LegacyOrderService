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

    public async Task<Result<Order>> ProcessOrderAsync(
        string customerName, 
        string productName, 
        long quantity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var product = await _productRepo.GetByNameAsync(productName, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductName}", productName);
                return Result<Order>.Failure($"Product '{productName}' does not exist.");
            }

            var order = new Order
            {
                CustomerName = customerName,
                ProductId = product.Id,
                Product = product,
                Quantity = quantity,
                Price = product.Price
            };
            await _orderRepo.SaveAsync(order, cancellationToken);

            double total = order.TotalAmount;
            _logger.LogInformation("Order processed successfully for Customer: {Customer}, Product: {Product}, Quantity: {Quantity}, Total Amount: ${TotalAmount}", 
                customerName, productName, quantity, total);

            return Result<Order>.Success(order);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError("Order processing was canceled: {ex}.", ex);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order for {CustomerName}", customerName);
            throw;
        }
    }
}
