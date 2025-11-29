using LegacyOrderService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService.Data;

public class ProductRepository : IProductRepository
{
    private readonly OrderDbContext _context;

    public ProductRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<double?> GetPriceAsync(string productName, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Name == productName, cancellationToken);

        return product?.Price;
    }
}
