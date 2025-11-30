using Bogus;
using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyOrderService.Tests;

public class ProductRepositoryTests
{
    private readonly ProductRepository _repo;
    private readonly Faker _faker;
    private readonly OrderDbContext _context;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        _context = new OrderDbContext(options);

        foreach (var item in ProductSeedData.Products)
        {
            _context.Products.Add(new Product
            {
                Name = item.Key,
                Price = item.Value
            });
        }
        _context.SaveChanges();

        _repo = new ProductRepository(_context);

        Randomizer.Seed = new Random(8675309);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetByName_GivenExistingProduct_ShouldReturnProductEntity()
    {
        // ARRANGE
        var availableProducts = ProductSeedData.Products.Keys.ToList();
        var productName = _faker.PickRandom(availableProducts);
        var expectedPrice = ProductSeedData.Products[productName];

        // ACT
        var result = await _repo.GetByNameAsync(productName, CancellationToken.None);

        // ASSERT
        Assert.NotNull(result);          
        Assert.Equal(productName, result.Name);
        Assert.Equal(expectedPrice, result.Price); 
        Assert.True(result.Id > 0);     
    }

    [Fact]
    public async Task GetByName_GivenNonExistentProduct_ShouldReturnNull()
    {
        // ARRANGE
        var randomProduct = _faker.Commerce.ProductName();

        // ACT
        var result = await _repo.GetByNameAsync(randomProduct, CancellationToken.None);

        // ASSERT
        Assert.Null(result); 
    }
}
