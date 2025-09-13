using UTGenDemo.Repository.Interfaces;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.Repository.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public InMemoryProductRepository()
    {
        _products = new List<Product>
        {
            new Product
            {
                Id = "1",
                Name = "Laptop",
                Price = 999.99m,
                Category = "Electronics",
                Stock = 15,
                IsDiscontinued = false
            },
            new Product
            {
                Id = "2",
                Name = "Coffee Mug",
                Price = 12.50m,
                Category = "Kitchen",
                Stock = 50,
                IsDiscontinued = false
            },
            new Product
            {
                Id = "3",
                Name = "Wireless Mouse",
                Price = 25.99m,
                Category = "Electronics",
                Stock = 0,
                IsDiscontinued = false
            },
            new Product
            {
                Id = "4",
                Name = "Old Keyboard",
                Price = 45.00m,
                Category = "Electronics",
                Stock = 5,
                IsDiscontinued = true
            },
            new Product
            {
                Id = "5",
                Name = "Desk Chair",
                Price = 199.99m,
                Category = "Furniture",
                Stock = 8,
                IsDiscontinued = false
            }
        };
    }

    public Task<Product?> GetProductByIdAsync(string productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        var products = _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(products);
    }

    public Task<IEnumerable<Product>> GetInStockProductsAsync()
    {
        var inStockProducts = _products.Where(p => p.IsInStock());
        return Task.FromResult(inStockProducts);
    }

    public Task<Product> CreateProductAsync(Product product)
    {
        // Generate a new ID
        var maxId = _products.Count > 0 ? _products.Max(p => int.Parse(p.Id)) : 0;
        product.Id = (maxId + 1).ToString();
        
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product> UpdateProductAsync(Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;
            existingProduct.Stock = product.Stock;
            existingProduct.IsDiscontinued = product.IsDiscontinued;
            return Task.FromResult(existingProduct);
        }
        
        throw new InvalidOperationException($"Product with ID {product.Id} not found");
    }

    public Task<bool> DeleteProductAsync(string productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            _products.Remove(product);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }
}