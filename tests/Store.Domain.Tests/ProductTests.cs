using Store.Domain.Entities;

namespace Store.Domain.Tests;

public class ProductTests
{
    [Theory]
    [InlineData(10, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsInStock_ShouldReflectStockQuantity(int stockQuantity, bool expected)
    {
        // Arrange
        var product = new Product
        {
            StockQuantity = stockQuantity
        };

        // Act
        var result = product.IsInStock;

        // Assert
        Assert.Equal(expected, result);
    }
}