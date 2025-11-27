using Bogus;
using LegacyOrderService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyOrderService.Tests;

public class ProductRepositoryTests
{
    private readonly ProductRepository _repo;
    private readonly Faker _faker;

    public ProductRepositoryTests()
    {
        _repo = new ProductRepository();
        Randomizer.Seed = new Random(8675309);
        _faker = new Faker();
    }

    [Fact]
    public void GetPrice_GivenExistingProduct_ShouldReturnCorrectPrice()
    {
        // ARRANGE
        var availableProducts = ProductSeedData.Products.Keys.ToList();
        var productName = _faker.PickRandom(availableProducts);
        var expectedPrice = ProductSeedData.Products[productName];

        // ACT
        var result = _repo.GetPrice(productName);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(expectedPrice, result);
    }

    [Fact]
    public void GetPrice_GivenNonExistentProduct_ShouldReturnNull()
    {
        // ARRANGE
        var randomProduct = _faker.Commerce.ProductName();

        // ACT
        var result = _repo.GetPrice(randomProduct);

        // ASSERT
        Assert.Null(result);
    }
}
