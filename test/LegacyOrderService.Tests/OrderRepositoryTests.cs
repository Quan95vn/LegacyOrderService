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
    private readonly Faker<Order> _orderFaker;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderDbContext(options);
        _repo = new OrderRepository(_context);

        Randomizer.Seed = new Random(8675309);
        _orderFaker = new Faker<Order>()
            .RuleFor(o => o.CustomerName, f => f.Name.FullName())
            .RuleFor(o => o.ProductName, f => f.Commerce.ProductName())
            .RuleFor(o => o.Quantity, f => f.Random.Long(1, 10))
            .RuleFor(o => o.Price, f => double.Parse(f.Commerce.Price(10, 100)));
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
        Assert.Equal(orderInput.ProductName, savedOrder.ProductName);
        Assert.Equal(orderInput.Price, savedOrder.Price);

        Assert.True(savedOrder.Id > 0);
    }
}
