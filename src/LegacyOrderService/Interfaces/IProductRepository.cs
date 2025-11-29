namespace LegacyOrderService.Interfaces;

public interface IProductRepository
{
    Task<double?> GetPriceAsync(string productName, CancellationToken cancellationToken = default);
}
