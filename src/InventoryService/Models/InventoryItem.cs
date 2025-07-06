namespace InventoryService.Models;

public class InventoryItem
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public int QuantityInStock { get; set; }
}