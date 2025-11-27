namespace LegacyOrderService.Interfaces;

public interface IProductRepository
{
    double? GetPrice(string productName);
}
