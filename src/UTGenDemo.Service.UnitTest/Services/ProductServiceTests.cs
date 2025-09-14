using Moq;
using Shouldly;
using UTGenDemo.Repository.Interfaces;
using UTGenDemo.Repository.Models;
using UTGenDemo.Service.Services;

namespace UTGenDemo.Service.UnitTest.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_productRepositoryMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenProductRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new ProductService(null!))
            .ParamName.ShouldBe("productRepository");
    }

    [Fact]
    public void Constructor_WhenProductRepositoryIsValid_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = new Mock<IProductRepository>();

        // Act
        var service = new ProductService(mockRepository.Object);

        // Assert
        service.ShouldNotBeNull();
    }

    #endregion

    #region GetProductByIdAsync Tests

    [Fact]
    public async Task GetProductByIdAsync_WhenProductIdIsValid_ShouldReturnProduct()
    {
        // Arrange
        var productId = "123";
        var expectedProduct = new Product { Id = productId, Name = "Test Product" };
        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldBe(expectedProduct);
        _productRepositoryMock.Verify(x => x.GetProductByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductNotFound_ShouldReturnNull()
    {
        // Arrange
        var productId = "nonexistent";
        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldBeNull();
        _productRepositoryMock.Verify(x => x.GetProductByIdAsync(productId), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetProductByIdAsync_WhenProductIdIsEmptyOrWhitespace_ShouldThrowArgumentException(string productId)
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetProductByIdAsync(productId));
        exception.ParamName.ShouldBe("productId");
        exception.Message.ShouldContain("Product ID cannot be null or empty");
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductIdIsNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetProductByIdAsync(null!));
        exception.ParamName.ShouldBe("productId");
        exception.Message.ShouldContain("Product ID cannot be null or empty");
    }

    #endregion

    #region GetAvailableProductsAsync Tests

    [Fact]
    public async Task GetAvailableProductsAsync_WhenProductsExist_ShouldReturnOnlyInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Product1", Stock = 5, IsDiscontinued = false },
            new() { Id = "2", Name = "Product2", Stock = 0, IsDiscontinued = false },
            new() { Id = "3", Name = "Product3", Stock = 3, IsDiscontinued = true },
            new() { Id = "4", Name = "Product4", Stock = 10, IsDiscontinued = false }
        };

        _productRepositoryMock.Setup(x => x.GetInStockProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAvailableProductsAsync();

        // Assert
        var availableProducts = result.ToList();
        availableProducts.Count.ShouldBe(2);
        availableProducts.ShouldContain(p => p.Id == "1");
        availableProducts.ShouldContain(p => p.Id == "4");
        _productRepositoryMock.Verify(x => x.GetInStockProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAvailableProductsAsync_WhenNoProductsInStock_ShouldReturnEmptyList()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Product1", Stock = 0, IsDiscontinued = false },
            new() { Id = "2", Name = "Product2", Stock = 5, IsDiscontinued = true }
        };

        _productRepositoryMock.Setup(x => x.GetInStockProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAvailableProductsAsync();

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(x => x.GetInStockProductsAsync(), Times.Once);
    }

    #endregion

    #region GetProductsByCategoryAsync Tests

    [Fact]
    public async Task GetProductsByCategoryAsync_WhenCategoryIsValid_ShouldReturnProducts()
    {
        // Arrange
        var category = "Electronics";
        var expectedProducts = new List<Product>
        {
            new() { Id = "1", Name = "Product1", Category = category },
            new() { Id = "2", Name = "Product2", Category = category }
        };

        _productRepositoryMock.Setup(x => x.GetProductsByCategoryAsync(category))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        result.ShouldBe(expectedProducts);
        _productRepositoryMock.Verify(x => x.GetProductsByCategoryAsync(category), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetProductsByCategoryAsync_WhenCategoryIsEmptyOrWhitespace_ShouldReturnEmptyList(string category)
    {
        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(x => x.GetProductsByCategoryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WhenCategoryIsNull_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetProductsByCategoryAsync(null!);

        // Assert
        result.ShouldBeEmpty();
        _productRepositoryMock.Verify(x => x.GetProductsByCategoryAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region CreateProductAsync Tests

    [Fact]
    public async Task CreateProductAsync_WhenProductIsValid_ShouldCreateProductWithId()
    {
        // Arrange
        var product = new Product
        {
            Name = "New Product",
            Price = 100,
            Category = "Electronics",
            Stock = 10
        };

        var expectedProduct = new Product
        {
            Id = "generated-id",
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Stock = product.Stock
        };

        _productRepositoryMock.Setup(x => x.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _sut.CreateProductAsync(product);

        // Assert
        result.ShouldBe(expectedProduct);
        product.Id.ShouldNotBeNullOrEmpty();
        _productRepositoryMock.Verify(x => x.CreateProductAsync(It.Is<Product>(p => !string.IsNullOrEmpty(p.Id))), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WhenProductIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentNullException>(() => _sut.CreateProductAsync(null!));
        exception.ParamName.ShouldBe("product");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateProductAsync_WhenProductNameIsEmptyOrWhitespace_ShouldThrowArgumentException(string name)
    {
        // Arrange
        var product = new Product
        {
            Name = name,
            Price = 100,
            Category = "Electronics"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product name is required");
    }

    [Fact]
    public async Task CreateProductAsync_WhenProductNameIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var product = new Product
        {
            Name = null!,
            Price = 100,
            Category = "Electronics"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product name is required");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task CreateProductAsync_WhenProductPriceIsNegative_ShouldThrowArgumentException(decimal price)
    {
        // Arrange
        var product = new Product
        {
            Name = "Valid Product",
            Price = price,
            Category = "Electronics"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateProductAsync(product));
        exception.ParamName.ShouldBe("product");
        exception.Message.ShouldContain("Product price cannot be negative");
    }

    #endregion

    #region CalculateTotalValue Tests

    [Fact]
    public void CalculateTotalValue_WhenProductsAreNull_ShouldReturnZero()
    {
        // Act
        var result = _sut.CalculateTotalValue(null!);

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public void CalculateTotalValue_WhenProductsListIsEmpty_ShouldReturnZero()
    {
        // Arrange
        var products = new List<Product>();

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public void CalculateTotalValue_WhenProductsAreInStock_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Price = 10, Stock = 5, IsDiscontinued = false }, // 50
            new() { Price = 20, Stock = 3, IsDiscontinued = false }, // 60
            new() { Price = 15, Stock = 2, IsDiscontinued = false }  // 30
        };

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(140); // 50 + 60 + 30
    }

    [Fact]
    public void CalculateTotalValue_WhenSomeProductsAreNotInStock_ShouldOnlyCalculateInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Price = 10, Stock = 5, IsDiscontinued = false }, // 50 - included
            new() { Price = 20, Stock = 0, IsDiscontinued = false }, // 0 - not in stock
            new() { Price = 15, Stock = 3, IsDiscontinued = true },  // 0 - discontinued
            new() { Price = 25, Stock = 4, IsDiscontinued = false }  // 100 - included
        };

        // Act
        var result = _sut.CalculateTotalValue(products);

        // Assert
        result.ShouldBe(150); // 50 + 100
    }

    #endregion

    #region ApplyDiscountAsync Tests

    [Fact]
    public async Task ApplyDiscountAsync_WhenProductExistsAndDiscountIsValid_ShouldApplyDiscountAndReturnTrue()
    {
        // Arrange
        var productId = "123";
        var discountPercentage = 20m;
        var originalPrice = 100m;
        var product = new Product 
        { 
            Id = productId, 
            Price = originalPrice, 
            Name = "Test Product" 
        };

        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync(product);
        _productRepositoryMock.Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, discountPercentage);

        // Assert
        result.ShouldBeTrue();
        product.Price.ShouldBe(80m); // 100 - (100 * 0.2)
        _productRepositoryMock.Verify(x => x.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task ApplyDiscountAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var productId = "nonexistent";
        var discountPercentage = 20m;

        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, discountPercentage);

        // Assert
        result.ShouldBeFalse();
        _productRepositoryMock.Verify(x => x.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(110)]
    public async Task ApplyDiscountAsync_WhenDiscountPercentageIsInvalid_ShouldReturnFalse(decimal invalidDiscount)
    {
        // Arrange
        var productId = "123";
        var product = new Product 
        { 
            Id = productId, 
            Price = 100m, 
            Name = "Test Product" 
        };

        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.ApplyDiscountAsync(productId, invalidDiscount);

        // Assert
        result.ShouldBeFalse();
        _productRepositoryMock.Verify(x => x.GetProductByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion
}