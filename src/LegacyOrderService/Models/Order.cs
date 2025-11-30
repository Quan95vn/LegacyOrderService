namespace LegacyOrderService.Models;

public class Order
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    
    public long ProductId { get; set; }
    // Navigation Property
    public Product Product { get; set; } = null!;

    public long Quantity { get; set; }
    public double Price { get; set; }

    public double TotalAmount => Price * Quantity;
}
