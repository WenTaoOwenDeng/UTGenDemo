using UTGenDemo.Repository.Models;
using UTGenDemo.Service.Interfaces;
using UTGenDemo.Repository.Interfaces;

namespace UTGenDemo.Service.Services;

public class ProductService: IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Product?> GetProductByIdAsync(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
        }

        return await _productRepository.GetProductByIdAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
    {
        var products = await _productRepository.GetInStockProductsAsync();
        return products.Where(p => p.IsInStock());
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Enumerable.Empty<Product>();
        }

        return await _productRepository.GetProductsByCategoryAsync(category);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Product name is required", nameof(product));
        }

        if (product.Price < 0)
        {
            throw new ArgumentException("Product price cannot be negative", nameof(product));
        }

        product.Id = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(product.Id))
        {
            throw new InvalidOperationException("Failed to generate product ID");
        }
        return await _productRepository.CreateProductAsync(product);
    }

    public decimal CalculateTotalValue(IEnumerable<Product> products)
    {
        if (products == null)
        {
            return 0;
        }

        return products.Where(p => p.IsInStock()).Sum(p => p.Price * p.Stock);
    }

    public async Task<bool> ApplyDiscountAsync(string productId, decimal discountPercentage)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            return false;
        }

        try
        {
            product.Price = product.CalculateDiscountPrice(discountPercentage);
            await _productRepository.UpdateProductAsync(product);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
