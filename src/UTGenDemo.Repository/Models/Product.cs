namespace UTGenDemo.Repository.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool IsDiscontinued { get; set; }
    
    public decimal CalculateDiscountPrice(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercentage), 
                "Discount percentage must be between 0 and 100");
        }
        
        return Price * (1 - discountPercentage / 100);
    }
    
    public bool IsInStock()
    {
        return Stock > 0 && !IsDiscontinued;
    }
}
