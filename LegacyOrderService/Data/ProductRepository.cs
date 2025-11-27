using LegacyOrderService.Interfaces;

namespace LegacyOrderService.Data;

public class ProductRepository : IProductRepository
{
    //private readonly Dictionary<string, double> _productPrices = new()
    //{
    //    ["Widget"] = 12.99,
    //    ["Gadget"] = 15.49,
    //    ["Doohickey"] = 8.75
    //};

    public double? GetPrice(string productName)
    {
        // Simulate an expensive lookup
        Thread.Sleep(500);

        if (ProductSeedData.Products.TryGetValue(productName, out var price))
        {
            return price;
        }

        return null;
    }
}
