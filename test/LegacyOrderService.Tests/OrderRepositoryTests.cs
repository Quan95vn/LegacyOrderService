using Bogus;
using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyOrderService.Tests;

public class OrderRepositoryTests
{
    private readonly OrderRepository _repo;
    private readonly OrderDbContext _context;
    private readonly Product _seedProduct;
    private readonly Faker<Order> _orderFaker;
    private readonly Faker _commonFaker;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderDbContext(options);
        _repo = new OrderRepository(_context);

        Randomizer.Seed = new Random(8675309);
        _commonFaker = new Faker();

        var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => double.Parse(f.Commerce.Price(10, 1000)));

        _seedProduct = productFaker.Generate();
        _context.Products.Add(_seedProduct);
        _context.SaveChanges();

        _orderFaker = new Faker<Order>()
            .RuleFor(o => o.CustomerName, f => f.Name.FullName())
            .RuleFor(o => o.ProductId, _seedProduct.Id)
            .RuleFor(o => o.Product, _seedProduct)
            .RuleFor(o => o.Quantity, f => f.Random.Long(1, 10))
            .RuleFor(o => o.Price, _seedProduct.Price);
    }

    [Fact]
    public async Task Save_ShouldPersistOrderToDatabase()
    {
        // ARRANGE
        var orderInput = _orderFaker.Generate();

        // ACT
        await _repo.SaveAsync(orderInput, It.IsAny<CancellationToken>());

        // ASSERT
        var savedOrder = await _context.Orders.FirstOrDefaultAsync();

        Assert.NotNull(savedOrder); 
        Assert.Equal(1, _context.Orders.Count());

        Assert.Equal(orderInput.CustomerName, savedOrder.CustomerName);
        Assert.Equal(orderInput.Price, savedOrder.Price);
        Assert.Equal(_seedProduct.Id, savedOrder.ProductId); 
        Assert.True(savedOrder.Id > 0);
    }

    [Fact]
    public async Task Save_ShouldHandleLargeQuantity_ExceedingInt32()
    {
        // ARRANGE
        var customerName = _commonFaker.Name.FullName();
        long hugeQuantity = _commonFaker.Random.Long(2_200_000_000, 5_000_000_000);

        var order = new Order
        {
            CustomerName = customerName,
            ProductId = _seedProduct.Id,
            Product = _seedProduct,
            Price = _seedProduct.Price,
            Quantity = hugeQuantity
        };

        // ACT
        await _repo.SaveAsync(order, CancellationToken.None);

        // ASSERT
        var savedOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.CustomerName == customerName);

        Assert.NotNull(savedOrder);
        Assert.Equal(hugeQuantity, savedOrder.Quantity);
    }
}
