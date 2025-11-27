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
    public void ProcessOrder_GivenValidProduct_ShouldReturnSuccess_AndSaveOrder()
    {
        // ARRANGE
        string customerName = _faker.Name.FullName();      
        string productName = _faker.Commerce.ProductName(); 
        int quantity = _faker.Random.Int(1, 10);          
        double price = double.Parse(_faker.Commerce.Price(10, 100));

        // Setup Mock
        _mockProductRepo.Setup(repo => repo.GetPrice(productName)).Returns(price);

        // ACT
        var result = _service.ProcessOrder(customerName, productName, quantity);

        // ASSERT
        Assert.True(result.IsSuccess);
        Assert.Equal(customerName, result.Value.CustomerName);

        double expectedTotal = price * quantity;
        Assert.Equal(expectedTotal, result.Value.Price * result.Value.Quantity);
    }

    [Fact]
    public void ProcessOrder_GivenNonExistentProduct_ShouldReturnFailure_AndNotSave()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName(); 
        var quantity = _faker.Random.Int(1, 10);

        // Setup Mock
        _mockProductRepo
            .Setup(repo => repo.GetPrice(productName))
            .Returns((double?)null);

        // ACT
        var result = _service.ProcessOrder(customerName, productName, quantity);

        // ASSERT
        Assert.False(result.IsSuccess);
        Assert.Contains(productName, result.ErrorMessage);
        _mockOrderRepo.Verify(repo => repo.Save(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public void ProcessOrder_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // ARRANGE
        var customerName = _faker.Name.FullName();
        var productName = _faker.Commerce.ProductName();
        var quantity = _faker.Random.Int(1, 5);
        var price = double.Parse(_faker.Commerce.Price(10, 500));

        var randomErrorMessage = _faker.Lorem.Sentence();

        _mockProductRepo
            .Setup(repo => repo.GetPrice(productName))
            .Returns(price);

       
        _mockOrderRepo
            .Setup(repo => repo.Save(It.IsAny<Order>()))
            .Throws(new Exception(randomErrorMessage));

        // ACT & ASSERT
        var ex = Assert.Throws<Exception>(() => _service.ProcessOrder(customerName, productName, quantity));
        Assert.Equal(randomErrorMessage, ex.Message);
    }
}
