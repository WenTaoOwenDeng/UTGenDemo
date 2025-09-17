using Xunit;
using Shouldly;
using UTGenDemo.Repository.Repositories;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.UnitTests.Repository.Repositories;

public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _sut;

    public InMemoryProductRepositoryTests()
    {
        _sut = new InMemoryProductRepository();
    }

    [Fact]
    public void Constructor_InitializesWithSeedData()
    {
        // Arrange & Act
        var repository = new InMemoryProductRepository();

        // Assert
        repository.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithExistingProductId_ReturnsProduct()
    {
        // Arrange
        var productId = "1"; // Seed data contains product with ID "1" (Laptop)

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId);
        result.Name.ShouldBe("Laptop");
        result.Price.ShouldBe(999.99m);
        result.Category.ShouldBe("Electronics");
        result.Stock.ShouldBe(15);
        result.IsDiscontinued.ShouldBeFalse();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNonExistentProductId_ReturnsNull()
    {
        // Arrange
        var productId = "999";

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNullProductId_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetProductByIdAsync(null!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithEmptyProductId_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetProductByIdAsync("");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithExistingCategory_ReturnsMatchingProducts()
    {
        // Arrange
        var category = "Electronics";

        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        var products = result.ToList();
        products.Count.ShouldBe(3); // Laptop, Wireless Mouse, Old Keyboard
        products.ShouldAllBe(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        products.ShouldContain(p => p.Name == "Laptop");
        products.ShouldContain(p => p.Name == "Wireless Mouse");
        products.ShouldContain(p => p.Name == "Old Keyboard");
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithCaseInsensitiveCategory_ReturnsMatchingProducts()
    {
        // Arrange
        var category = "ELECTRONICS"; // Different case

        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        var products = result.ToList();
        products.Count.ShouldBe(3); // Should still match Electronics products
        products.ShouldAllBe(p => p.Category.Equals("Electronics", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithNonExistentCategory_ReturnsEmptyCollection()
    {
        // Arrange
        var category = "NonExistentCategory";

        // Act
        var result = await _sut.GetProductsByCategoryAsync(category);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithNullCategory_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await _sut.GetProductsByCategoryAsync(null!);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithEmptyCategory_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await _sut.GetProductsByCategoryAsync("");

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetInStockProductsAsync_ReturnsOnlyInStockProducts()
    {
        // Act
        var result = await _sut.GetInStockProductsAsync();

        // Assert
        var inStockProducts = result.ToList();
        inStockProducts.Count.ShouldBe(3); // Laptop, Coffee Mug, Desk Chair (based on seed data)
        inStockProducts.ShouldAllBe(p => p.IsInStock());
        inStockProducts.ShouldContain(p => p.Name == "Laptop");
        inStockProducts.ShouldContain(p => p.Name == "Coffee Mug");
        inStockProducts.ShouldContain(p => p.Name == "Desk Chair");
        inStockProducts.ShouldNotContain(p => p.Name == "Wireless Mouse"); // Out of stock
        inStockProducts.ShouldNotContain(p => p.Name == "Old Keyboard"); // Discontinued
    }

    [Fact]
    public async Task CreateProductAsync_WithValidProduct_CreatesProductWithGeneratedId()
    {
        // Arrange
        var product = new Product
        {
            Name = "New Product",
            Price = 199.99m,
            Category = "Test Category",
            Stock = 10,
            IsDiscontinued = false
        };

        // Act
        var result = await _sut.CreateProductAsync(product);

        // Assert
        result.ShouldBe(product);
        product.Id.ShouldNotBeNullOrWhiteSpace();
        product.Id.ShouldNotBe("0");

        // Verify product is actually added to repository
        var retrievedProduct = await _sut.GetProductByIdAsync(product.Id);
        retrievedProduct.ShouldBe(product);
    }

    [Fact]
    public async Task CreateProductAsync_MultipleProducts_GeneratesUniqueIds()
    {
        // Arrange
        var product1 = new Product { Name = "Product 1", Price = 100m, Category = "Test" };
        var product2 = new Product { Name = "Product 2", Price = 200m, Category = "Test" };

        // Act
        var result1 = await _sut.CreateProductAsync(product1);
        var result2 = await _sut.CreateProductAsync(product2);

        // Assert
        result1.Id.ShouldNotBe(result2.Id);
        result1.Id.ShouldNotBeNullOrWhiteSpace();
        result2.Id.ShouldNotBeNullOrWhiteSpace();
        int.Parse(result2.Id).ShouldBeGreaterThan(int.Parse(result1.Id));
    }

    [Fact]
    public async Task UpdateProductAsync_WithExistingProduct_UpdatesProductProperties()
    {
        // Arrange
        var existingProductId = "1"; // From seed data (Laptop)
        var originalProduct = await _sut.GetProductByIdAsync(existingProductId);
        var updatedProduct = new Product
        {
            Id = existingProductId,
            Name = "Updated Laptop",
            Price = 1299.99m,
            Category = "Updated Electronics",
            Stock = 25,
            IsDiscontinued = true
        };

        // Act
        var result = await _sut.UpdateProductAsync(updatedProduct);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingProductId);
        result.Name.ShouldBe("Updated Laptop");
        result.Price.ShouldBe(1299.99m);
        result.Category.ShouldBe("Updated Electronics");
        result.Stock.ShouldBe(25);
        result.IsDiscontinued.ShouldBeTrue();

        // Verify the original product reference was updated
        originalProduct!.Name.ShouldBe("Updated Laptop");
        originalProduct.Price.ShouldBe(1299.99m);
        originalProduct.Category.ShouldBe("Updated Electronics");
        originalProduct.Stock.ShouldBe(25);
        originalProduct.IsDiscontinued.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateProductAsync_WithNonExistentProduct_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentProduct = new Product
        {
            Id = "999",
            Name = "Non-existent Product",
            Price = 100m,
            Category = "Test"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _sut.UpdateProductAsync(nonExistentProduct));
        exception.Message.ShouldContain("Product with ID 999 not found");
    }

    [Fact]
    public async Task DeleteProductAsync_WithExistingProductId_DeletesProductAndReturnsTrue()
    {
        // Arrange
        var productId = "2"; // From seed data (Coffee Mug)

        // Verify product exists before deletion
        var productBeforeDelete = await _sut.GetProductByIdAsync(productId);
        productBeforeDelete.ShouldNotBeNull();

        // Act
        var result = await _sut.DeleteProductAsync(productId);

        // Assert
        result.ShouldBeTrue();

        // Verify product no longer exists
        var productAfterDelete = await _sut.GetProductByIdAsync(productId);
        productAfterDelete.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteProductAsync_WithNonExistentProductId_ReturnsFalse()
    {
        // Arrange
        var nonExistentProductId = "999";

        // Act
        var result = await _sut.DeleteProductAsync(nonExistentProductId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteProductAsync_WithNullProductId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.DeleteProductAsync(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteProductAsync_WithEmptyProductId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.DeleteProductAsync("");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task Repository_MaintainsDataConsistency_AfterMultipleOperations()
    {
        // Arrange
        var newProduct = new Product 
        { 
            Name = "Test Product", 
            Price = 99.99m, 
            Category = "Test", 
            Stock = 5 
        };

        // Act & Assert - Create
        var createdProduct = await _sut.CreateProductAsync(newProduct);
        createdProduct.ShouldNotBeNull();

        // Act & Assert - Retrieve
        var retrievedProduct = await _sut.GetProductByIdAsync(createdProduct.Id);
        retrievedProduct.ShouldBe(createdProduct);

        // Act & Assert - Update
        createdProduct.Name = "Updated Test Product";
        createdProduct.Price = 149.99m;
        var updatedProduct = await _sut.UpdateProductAsync(createdProduct);
        updatedProduct.Name.ShouldBe("Updated Test Product");
        updatedProduct.Price.ShouldBe(149.99m);

        // Act & Assert - Retrieve after update
        var retrievedAfterUpdate = await _sut.GetProductByIdAsync(createdProduct.Id);
        retrievedAfterUpdate!.Name.ShouldBe("Updated Test Product");
        retrievedAfterUpdate.Price.ShouldBe(149.99m);

        // Act & Assert - Delete
        var deleted = await _sut.DeleteProductAsync(createdProduct.Id);
        deleted.ShouldBeTrue();

        // Act & Assert - Verify deletion
        var deletedProduct = await _sut.GetProductByIdAsync(createdProduct.Id);
        deletedProduct.ShouldBeNull();
    }

    [Fact]
    public async Task SeedData_ContainsExpectedProducts()
    {
        // This test verifies the seed data is correctly initialized
        
        // Act
        var laptop = await _sut.GetProductByIdAsync("1");
        var coffeeMug = await _sut.GetProductByIdAsync("2");
        var wirelessMouse = await _sut.GetProductByIdAsync("3");
        var oldKeyboard = await _sut.GetProductByIdAsync("4");
        var deskChair = await _sut.GetProductByIdAsync("5");

        // Assert
        laptop.ShouldNotBeNull();
        laptop.Name.ShouldBe("Laptop");
        laptop.Price.ShouldBe(999.99m);
        laptop.Category.ShouldBe("Electronics");
        laptop.Stock.ShouldBe(15);
        laptop.IsDiscontinued.ShouldBeFalse();

        coffeeMug.ShouldNotBeNull();
        coffeeMug.Name.ShouldBe("Coffee Mug");
        coffeeMug.Price.ShouldBe(12.50m);
        coffeeMug.Category.ShouldBe("Kitchen");
        coffeeMug.Stock.ShouldBe(50);
        coffeeMug.IsDiscontinued.ShouldBeFalse();

        wirelessMouse.ShouldNotBeNull();
        wirelessMouse.Name.ShouldBe("Wireless Mouse");
        wirelessMouse.Price.ShouldBe(25.99m);
        wirelessMouse.Category.ShouldBe("Electronics");
        wirelessMouse.Stock.ShouldBe(0); // Out of stock
        wirelessMouse.IsDiscontinued.ShouldBeFalse();

        oldKeyboard.ShouldNotBeNull();
        oldKeyboard.Name.ShouldBe("Old Keyboard");
        oldKeyboard.Price.ShouldBe(45.00m);
        oldKeyboard.Category.ShouldBe("Electronics");
        oldKeyboard.Stock.ShouldBe(5);
        oldKeyboard.IsDiscontinued.ShouldBeTrue(); // Discontinued

        deskChair.ShouldNotBeNull();
        deskChair.Name.ShouldBe("Desk Chair");
        deskChair.Price.ShouldBe(199.99m);
        deskChair.Category.ShouldBe("Furniture");
        deskChair.Stock.ShouldBe(8);
        deskChair.IsDiscontinued.ShouldBeFalse();
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithMultipleCategories_ReturnsCorrectCounts()
    {
        // Act
        var electronicsProducts = await _sut.GetProductsByCategoryAsync("Electronics");
        var kitchenProducts = await _sut.GetProductsByCategoryAsync("Kitchen");
        var furnitureProducts = await _sut.GetProductsByCategoryAsync("Furniture");

        // Assert
        electronicsProducts.Count().ShouldBe(3); // Laptop, Wireless Mouse, Old Keyboard
        kitchenProducts.Count().ShouldBe(1); // Coffee Mug
        furnitureProducts.Count().ShouldBe(1); // Desk Chair
    }

    [Fact]
    public async Task GetInStockProductsAsync_ExcludesOutOfStockAndDiscontinued()
    {
        // Act
        var inStockProducts = await _sut.GetInStockProductsAsync();

        // Assert
        var products = inStockProducts.ToList();
        
        // Should include products with stock > 0 and not discontinued
        products.ShouldContain(p => p.Name == "Laptop"); // Stock: 15, Not discontinued
        products.ShouldContain(p => p.Name == "Coffee Mug"); // Stock: 50, Not discontinued
        products.ShouldContain(p => p.Name == "Desk Chair"); // Stock: 8, Not discontinued
        
        // Should exclude out of stock products
        products.ShouldNotContain(p => p.Name == "Wireless Mouse"); // Stock: 0
        
        // Should exclude discontinued products (even if they have stock)
        products.ShouldNotContain(p => p.Name == "Old Keyboard"); // Discontinued: true
    }
}