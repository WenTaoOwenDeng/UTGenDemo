using Xunit;
using Moq;
using Shouldly;
using UTGenDemo.Service.Services;
using UTGenDemo.Repository.Models;
using UTGenDemo.Repository.Interfaces;

namespace UTGenDemo.UnitTests.Service.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public void Constructor_WithNullProductRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => new ProductService(null!));
        exception.ParamName.ShouldBe("productRepository");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new ProductService(_productRepositoryMock.Object);

        // Assert
        service.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNullProductId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetProductByIdAsync(null!));
        exception.ParamName.ShouldBe("productId");
        exception.Message.ShouldContain("Product ID cannot be null or empty");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithEmptyProductId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetProductByIdAsync(""));
        exception.ParamName.ShouldBe("productId");
        exception.Message.ShouldContain("Product ID cannot be null or empty");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithWhitespaceProductId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetProductByIdAsync("   "));
        exception.ParamName.ShouldBe("productId");
        exception.Message.ShouldContain("Product ID cannot be null or empty");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidProductId_ProductExists_ReturnsProduct()
    {
        // Arrange
        var productId = "123";
        var expectedProduct = new Product { Id = productId, Name = "Test Product" };
        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldBe(expectedProduct);
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidProductId_ProductNotExists_ReturnsNull()
    {
        // Arrange
        var productId = "123";
        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldBeNull();
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAvailableProductsAsync_FiltersInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = "1", Name = "In Stock", Stock = 5, IsDiscontinued = false },
            new Product { Id = "2", Name = "Out of Stock", Stock = 0, IsDiscontinued = false },
            new Product { Id = "3", Name = "Discontinued", Stock = 3, IsDiscontinued = true },
            new Product { Id = "4", Name = "Available", Stock = 10, IsDiscontinued = false }
        };
        _productRepositoryMock.Setup(r => r.GetInStockProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAvailableProductsAsync();

        // Assert
        var availableProducts = result.ToList();
        availableProducts.Count.ShouldBe(2);
        availableProducts.All(p => p.IsInStock()).ShouldBeTrue();
        availableProducts.ShouldContain(p => p.Id == "1");
        availableProducts.ShouldContain(p => p.Id == "4");
        _productRepositoryMock.Verify(r => r.GetInStockProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAvailableProductsAsync_WithNoInStockProducts_ReturnsEmptyCollection()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = "1", Name = "Out of Stock", Stock = 0, IsDiscontinued = false },
            new Product { Id = "2", Name = "Discontinued", Stock = 3, IsDiscontinued = true }
        };
        _productRepositoryMock.Setup(r => r.GetInStockProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAvailableProductsAsync();

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(r => r.GetInStockProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithNullCategory_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await _sut.GetProductsByCategoryAsync(null!);

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(r => r.GetProductsByCategoryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithEmptyCategory_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await _sut.GetProductsByCategoryAsync("");

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(r => r.GetProductsByCategoryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithWhitespaceCategory_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await _sut.GetProductsByCategoryAsync("   ");

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(r => r.GetProductsByCategoryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithValidCategory_ReturnsProducts()
    {
        // Arrange
        var category = "Electronics";
        var products = new List<Product>
        {
            new Product { Id = "1", Name = "Laptop", Category = "Electronics" },
            new Product { Id = "2", Name = "Mouse", Category = "Electronics" }
        };
        _productRepositoryMock.Setup(r => r.GetProductsByCategoryAsync(category))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        result.ShouldBe(products);
        _productRepositoryMock.Verify(r => r.GetProductsByCategoryAsync(category), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WithNullProduct_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentNullException>(() => _sut.CreateProductAsync(null!));
        exception.ParamName.ShouldBe("product");
    }

    [Fact]
    public async Task CreateProductAsync_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var product = new Product { Name = null!, Price = 100m };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product name is required");
    }

    [Fact]
    public async Task CreateProductAsync_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var product = new Product { Name = "", Price = 100m };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product name is required");
    }

    [Fact]
    public async Task CreateProductAsync_WithWhitespaceName_ThrowsArgumentException()
    {
        // Arrange
        var product = new Product { Name = "   ", Price = 100m };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product name is required");
    }

    [Fact]
    public async Task CreateProductAsync_WithNegativePrice_ThrowsArgumentException()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Price = -10m };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product price cannot be negative");
    }

    [Fact]
    public async Task CreateProductAsync_WithValidProduct_CreatesProductWithGeneratedId()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Price = 100m };
        var createdProduct = new Product { Id = "123", Name = "Test Product", Price = 100m };

        _productRepositoryMock.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _sut.CreateProductAsync(product);

        // Assert
        result.ShouldBe(createdProduct);
        product.Id.ShouldNotBeNullOrWhiteSpace();
        _productRepositoryMock.Verify(r => r.CreateProductAsync(It.Is<Product>(p => 
            p.Id != null && 
            p.Name == product.Name && 
            p.Price == product.Price)), Times.Once);
    }

    [Fact]
    public void CalculateTotalValue_WithNullProducts_ReturnsZero()
    {
        // Arrange & Act
        var result = _sut.CalculateTotalValue(null!);

        // Assert
        result.ShouldBe(0m);
    }

    [Fact]
    public void CalculateTotalValue_WithEmptyProducts_ReturnsZero()
    {
        // Arrange
        var products = new List<Product>();

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(0m);
    }

    [Fact]
    public void CalculateTotalValue_WithInStockProducts_CalculatesCorrectTotal()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Price = 100m, Stock = 5, IsDiscontinued = false }, // 500
            new Product { Price = 50m, Stock = 3, IsDiscontinued = false },  // 150
            new Product { Price = 200m, Stock = 2, IsDiscontinued = false }  // 400
        };

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(1050m); // 500 + 150 + 400
    }

    [Fact]
    public void CalculateTotalValue_WithMixedProducts_OnlyCalculatesInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Price = 100m, Stock = 5, IsDiscontinued = false }, // 500 (in stock)
            new Product { Price = 50m, Stock = 0, IsDiscontinued = false },  // 0 (out of stock)
            new Product { Price = 200m, Stock = 2, IsDiscontinued = true }   // 0 (discontinued)
        };

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(500m); // Only the in-stock product
    }

    [Fact]
    public async Task ApplyDiscountAsync_WithNonExistentProduct_ReturnsFalse()
    {
        // Arrange
        var productId = "123";
        var discountPercentage = 10m;
        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, discountPercentage);

        // Assert
        result.ShouldBeFalse();
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ApplyDiscountAsync_WithValidProductAndDiscount_AppliesDiscountAndReturnsTrue()
    {
        // Arrange
        var productId = "123";
        var discountPercentage = 10m;
        var originalPrice = 100m;
        var product = new Product { Id = productId, Name = "Test Product", Price = originalPrice };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, discountPercentage);

        // Assert
        result.ShouldBeTrue();
        product.Price.ShouldBe(90m); // 100 - 10%
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task ApplyDiscountAsync_WithInvalidDiscountPercentage_ReturnsFalse()
    {
        // Arrange
        var productId = "123";
        var invalidDiscountPercentage = 150m; // Invalid discount > 100%
        var product = new Product { Id = productId, Name = "Test Product", Price = 100m };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, invalidDiscountPercentage);

        // Assert
        result.ShouldBeFalse();
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ApplyDiscountAsync_WithNegativeDiscountPercentage_ReturnsFalse()
    {
        // Arrange
        var productId = "123";
        var negativeDiscountPercentage = -10m;
        var product = new Product { Id = productId, Name = "Test Product", Price = 100m };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, negativeDiscountPercentage);

        // Assert
        result.ShouldBeFalse();
        _productRepositoryMock.Verify(r => r.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }
}