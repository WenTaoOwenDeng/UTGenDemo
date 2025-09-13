using UTGenDemo.Repository.Models;

namespace UTGenDemo.Service.Interfaces;

public interface IProductService
{
    Task<Product?> GetProductByIdAsync(string productId);
    Task<IEnumerable<Product>> GetAvailableProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> ApplyDiscountAsync(string productId, decimal discountPercentage);
    decimal CalculateTotalValue(IEnumerable<Product> products);
}