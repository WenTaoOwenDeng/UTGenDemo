using Xunit;
using Shouldly;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.UnitTests.Repository.Models;

public class ProductTests
{
    [Fact]
    public void Constructor_CreatesProductWithDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.Id.ShouldBe(string.Empty);
        product.Name.ShouldBe(string.Empty);
        product.Price.ShouldBe(0m);
        product.Category.ShouldBe(string.Empty);
        product.Stock.ShouldBe(0);
        product.IsDiscontinued.ShouldBeFalse(); // Default value is false
    }

    [Fact]
    public void CalculateDiscountPrice_WithValidDiscountPercentage_ReturnsDiscountedPrice()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var discountPercentage = 10m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(90m); // 100 - 10%
    }

    [Fact]
    public void CalculateDiscountPrice_WithZeroDiscountPercentage_ReturnsOriginalPrice()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var discountPercentage = 0m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(100m); // No discount
    }

    [Fact]
    public void CalculateDiscountPrice_WithMaximumDiscountPercentage_ReturnsZeroPrice()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var discountPercentage = 100m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(0m); // 100% discount
    }

    [Fact]
    public void CalculateDiscountPrice_WithFiftyPercentDiscount_ReturnsHalfPrice()
    {
        // Arrange
        var product = new Product { Price = 200m };
        var discountPercentage = 50m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(100m); // 50% discount
    }

    [Fact]
    public void CalculateDiscountPrice_WithNegativeDiscountPercentage_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var negativeDiscountPercentage = -10m;

        // Act & Assert
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => product.CalculateDiscountPrice(negativeDiscountPercentage));
        exception.ParamName.ShouldBe("discountPercentage");
        exception.Message.ShouldContain("Discount percentage must be between 0 and 100");
    }

    [Fact]
    public void CalculateDiscountPrice_WithDiscountPercentageAbove100_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var invalidDiscountPercentage = 150m;

        // Act & Assert
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => product.CalculateDiscountPrice(invalidDiscountPercentage));
        exception.ParamName.ShouldBe("discountPercentage");
        exception.Message.ShouldContain("Discount percentage must be between 0 and 100");
    }

    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(100, 25, 75)]
    [InlineData(100, 50, 50)]
    [InlineData(200, 25, 150)]
    public void CalculateDiscountPrice_WithVariousDiscountPercentages_ReturnsCorrectDiscountedPrice(double originalPrice, double discountPercentage, double expectedPrice)
    {
        // Arrange
        var product = new Product { Price = (decimal)originalPrice };

        // Act
        var discountedPrice = product.CalculateDiscountPrice((decimal)discountPercentage);

        // Assert
        discountedPrice.ShouldBe((decimal)expectedPrice);
    }

    [Fact]
    public void IsInStock_WithPositiveStockAndNotDiscontinued_ReturnsTrue()
    {
        // Arrange
        var product = new Product 
        { 
            Stock = 5, 
            IsDiscontinued = false 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBeTrue();
    }

    [Fact]
    public void IsInStock_WithZeroStockAndNotDiscontinued_ReturnsFalse()
    {
        // Arrange
        var product = new Product 
        { 
            Stock = 0, 
            IsDiscontinued = false 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBeFalse();
    }

    [Fact]
    public void IsInStock_WithPositiveStockButDiscontinued_ReturnsFalse()
    {
        // Arrange
        var product = new Product 
        { 
            Stock = 10, 
            IsDiscontinued = true 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBeFalse();
    }

    [Fact]
    public void IsInStock_WithZeroStockAndDiscontinued_ReturnsFalse()
    {
        // Arrange
        var product = new Product 
        { 
            Stock = 0, 
            IsDiscontinued = true 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBeFalse();
    }

    [Fact]
    public void IsInStock_WithNegativeStock_ReturnsFalse()
    {
        // Arrange
        var product = new Product 
        { 
            Stock = -1, 
            IsDiscontinued = false 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBeFalse();
    }

    [Theory]
    [InlineData(1, false, true)]
    [InlineData(100, false, true)]
    [InlineData(0, false, false)]
    [InlineData(-1, false, false)]
    [InlineData(1, true, false)]
    [InlineData(100, true, false)]
    [InlineData(0, true, false)]
    [InlineData(-1, true, false)]
    public void IsInStock_WithVariousStockAndDiscontinuedCombinations_ReturnsExpectedResult(int stock, bool isDiscontinued, bool expectedInStock)
    {
        // Arrange
        var product = new Product 
        { 
            Stock = stock, 
            IsDiscontinued = isDiscontinued 
        };

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.ShouldBe(expectedInStock);
    }

    [Fact]
    public void Product_AllPropertiesCanBeSetAndRetrieved()
    {
        // Arrange
        var id = "123";
        var name = "Test Product";
        var price = 99.99m;
        var category = "Test Category";
        var stock = 10;
        var isDiscontinued = true;

        // Act
        var product = new Product
        {
            Id = id,
            Name = name,
            Price = price,
            Category = category,
            Stock = stock,
            IsDiscontinued = isDiscontinued
        };

        // Assert
        product.Id.ShouldBe(id);
        product.Name.ShouldBe(name);
        product.Price.ShouldBe(price);
        product.Category.ShouldBe(category);
        product.Stock.ShouldBe(stock);
        product.IsDiscontinued.ShouldBe(isDiscontinued);
    }

    [Fact]
    public void CalculateDiscountPrice_WithDecimalPrice_HandlesDecimalAccurately()
    {
        // Arrange
        var product = new Product { Price = 123.45m };
        var discountPercentage = 15m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(104.9325m); // 123.45 * (1 - 0.15)
    }

    [Fact]
    public void CalculateDiscountPrice_WithZeroPrice_ReturnsZero()
    {
        // Arrange
        var product = new Product { Price = 0m };
        var discountPercentage = 50m;

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(0m);
    }

    [Fact]
    public void CalculateDiscountPrice_WithVerySmallDiscount_ReturnsNearOriginalPrice()
    {
        // Arrange
        var product = new Product { Price = 100m };
        var discountPercentage = 0.01m; // 0.01%

        // Act
        var discountedPrice = product.CalculateDiscountPrice(discountPercentage);

        // Assert
        discountedPrice.ShouldBe(99.9900m); // Actual calculation: 100 * (1 - 0.0001)
    }

    [Fact]
    public void Product_IsDiscontinued_DefaultsToFalse()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.IsDiscontinued.ShouldBeFalse();
    }

    [Fact]
    public void Product_CanBeDiscontinued()
    {
        // Arrange
        var product = new Product { IsDiscontinued = false };

        // Act
        product.IsDiscontinued = true;

        // Assert
        product.IsDiscontinued.ShouldBeTrue();
    }

    [Fact]
    public void Product_WithSpecialCharactersInName_HandlesCorrectly()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "Product with Spécial Chäractërs & Symbols!@#$%"
        };

        // Assert
        product.Name.ShouldBe("Product with Spécial Chäractërs & Symbols!@#$%");
    }

    [Fact]
    public void Product_WithVeryLargePrice_HandlesCorrectly()
    {
        // Arrange & Act
        var product = new Product { Price = decimal.MaxValue };

        // Assert
        product.Price.ShouldBe(decimal.MaxValue);
    }

    [Fact]
    public void Product_WithVeryLargeStock_HandlesCorrectly()
    {
        // Arrange & Act
        var product = new Product { Stock = int.MaxValue };

        // Assert
        product.Stock.ShouldBe(int.MaxValue);
    }
}