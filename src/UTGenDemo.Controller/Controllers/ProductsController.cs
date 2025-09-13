using Microsoft.AspNetCore.Mvc;
using UTGenDemo.Repository.Models;
using UTGenDemo.Service.Interfaces;

namespace UTGenDemo.Controller.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a product by its ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Get all available products (in stock and not discontinued)
    /// </summary>
    /// <returns>List of available products</returns>
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Product>>> GetAvailableProducts()
    {
        var products = await _productService.GetAvailableProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get all products in a specific category
    /// </summary>
    /// <param name="category">Product category</param>
    /// <returns>List of products in the category</returns>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
    {
        var products = await _productService.GetProductsByCategoryAsync(category);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="product">Product details</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Apply a discount to a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="discountRequest">Discount details</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/discount")]
    public async Task<ActionResult> ApplyDiscount(string id, [FromBody] ApplyDiscountRequest discountRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _productService.ApplyDiscountAsync(id, discountRequest.DiscountPercentage);
        if (!success)
        {
            return NotFound($"Product with ID {id} not found or discount percentage is invalid");
        }

        return Ok(new { message = "Discount applied successfully" });
    }

    /// <summary>
    /// Calculate total value of provided products
    /// </summary>
    /// <param name="productIds">List of product IDs</param>
    /// <returns>Total value calculation</returns>
    [HttpPost("calculate-total-value")]
    public async Task<ActionResult<decimal>> CalculateTotalValue([FromBody] IEnumerable<string> productIds)
    {
        var products = new List<Product>();
        
        foreach (var productId in productIds)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null)
            {
                products.Add(product);
            }
        }

        var totalValue = _productService.CalculateTotalValue(products);
        return Ok(new { totalValue, productCount = products.Count });
    }
}

/// <summary>
/// Request model for applying discount
/// </summary>
public class ApplyDiscountRequest
{
    public decimal DiscountPercentage { get; set; }
}