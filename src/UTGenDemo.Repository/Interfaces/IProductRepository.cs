using UTGenDemo.Repository.Models;

namespace UTGenDemo.Repository.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(string productId);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetInStockProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(string productId);
}