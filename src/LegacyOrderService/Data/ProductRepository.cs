using LegacyOrderService.Interfaces;
using LegacyOrderService.Models;
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

    public async Task<Product?> GetByNameAsync(string productName, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower(), cancellationToken);
    }
}
