using LegacyOrderService.Interfaces;

namespace LegacyOrderService.Data;

public class ProductRepository : IProductRepository
{
    private readonly OrderDbContext _context;

    public ProductRepository(OrderDbContext context)
    {
        _context = context;
    }

    public double? GetPrice(string productName)
    {
        var product = _context.Products
                .FirstOrDefault(p => p.Name.ToLower() == productName.ToLower());

        return product?.Price;
    }
}
