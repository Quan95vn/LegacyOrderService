using LegacyOrderService.Models;

namespace LegacyOrderService.Interfaces;

public interface IProductRepository
{
    Task<double?> GetPriceAsync(string productName, CancellationToken cancellationToken = default);
    Task<Product?> GetByNameAsync(string productName, CancellationToken cancellationToken = default);
}
