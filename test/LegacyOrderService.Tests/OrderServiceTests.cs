using Bogus;
using LegacyOrderService.Interfaces;
using LegacyOrderService.Models;
using LegacyOrderService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LegacyOrderService.Tests;

public class OrderServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<ILogger<OrderService>> _mockLogger;

    private readonly OrderService _service;
    private readonly Faker _faker;

    public OrderServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();

        _service = new OrderService(
            _mockProductRepo.Object,
            _mockOrderRepo.Object,
            _mockLogger.Object
        );

        Randomizer.Seed = new Random(8675309);
        _faker = new Faker();
    }

    [Fact]
    public async Task ProcessOrder_GivenValidProduct_ShouldReturnSuccess_AndSaveOrder()
    {
        // ARRANGE
        string customerName = _faker.Name.FullName();
        string productName = _faker.Commerce.ProductName();
        long quantity = _faker.Random.Long(1, 10);
        double price = double.Parse(_faker.Commerce.Price(10, 100));

        var mockProduct = new Product
        {
            Id = 123, 
            Name = productName,
            Price = price
        };

        // Setup Mock
        _mockProductRepo
                .Setup(repo => repo.GetByNameAsync(productName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockProduct);

        // ACT
        var result = await _service.ProcessOrderAsync(customerName, productName, quantity);

        // ASSERT
        Assert.True(result.IsSuccess);

        Assert.Equal(customerName, result.Value.CustomerName);
        Assert.Equal(mockProduct.Id, result.Value.ProductId); 
        Assert.Equal(mockProduct.Price, result.Value.Price);

        Assert.NotNull(result.Value.Product);
        Assert.Equal(productName, result.Value.Product.Name);

        double expectedTotal = price * quantity;
        Assert.Equal(expectedTotal, result.Value.Price * result.Value.Quantity);

        _mockOrderRepo.Verify(repo => repo.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessOrder_GivenNonExistentProduct_ShouldReturnFailure_AndNotSave()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName();
        var quantity = _faker.Random.Long(1, 10);

        // Setup Mock
        _mockProductRepo
            .Setup(repo => repo.GetPriceAsync(productName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((double?)null);

        // ACT
        var result = await _service.ProcessOrderAsync(customerName, productName, quantity, It.IsAny<CancellationToken>());

        // ASSERT
        Assert.False(result.IsSuccess);
        Assert.Contains(productName, result.ErrorMessage);

        _mockOrderRepo.Verify(repo => repo.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessOrder_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName();
        var quantity = _faker.Random.Long(1, 5);
        var price = double.Parse(_faker.Commerce.Price(10, 500));

        var randomErrorMessage = _faker.Lorem.Sentence();
        var mockProduct = new Product { 
            Id = 1, 
            Name = productName, 
            Price = price 
        };

        _mockProductRepo
            .Setup(repo => repo.GetByNameAsync(productName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockProduct);

        _mockOrderRepo
            .Setup(repo => repo.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(randomErrorMessage));

        // ACT & ASSERT
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.ProcessOrderAsync(customerName, productName, quantity));

        Assert.Equal(randomErrorMessage, ex.Message);
    }

    [Fact]
    public async Task ProcessOrder_ShouldPassCancellationTokenToDependencies()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName();
        var quantity = _faker.Random.Long(1, 5);
        var price = double.Parse(_faker.Commerce.Price(10, 500));

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var mockProduct = new Product { 
            Id = 1, 
            Name = productName, 
            Price = price 
        };

        _mockProductRepo
            .Setup(repo => repo.GetByNameAsync(productName, token)) 
            .ReturnsAsync(mockProduct);

        // ACT
        await _service.ProcessOrderAsync(customerName, productName, quantity, token);

        // ASSERT
        _mockProductRepo.Verify(repo => repo.GetByNameAsync(productName, token), Times.Once);
        _mockOrderRepo.Verify(repo => repo.SaveAsync(It.IsAny<Order>(), token), Times.Once);
    }

    [Fact]
    public async Task ProcessOrder_WhenTokenIsCancelled_ShouldThrowException()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName();
        var quantity = _faker.Random.Long(1, 5);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        //_mockProductRepo
        //    .Setup(repo => repo.GetPriceAsync(It.IsAny<string>(), cts.Token))
        //    .Throws(new OperationCanceledException());

        // ACT & ASSERT
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _service.ProcessOrderAsync(customerName, productName, quantity, cts.Token));

        _mockOrderRepo.Verify(repo => repo.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
