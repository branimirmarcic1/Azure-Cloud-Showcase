using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Models;
using Contracts;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public InventoryController(InventoryDbContext context)
    {
        _context = context;
    }

    // GET: api/inventory/{productName}
    [HttpGet("{productName}")]
    public async Task<ActionResult<InventoryItem>> GetInventory(string productName)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductName.ToLower() == productName.ToLower());

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    // **NOVA METODA**
    // POST: api/inventory/reduce
    [HttpPost("reduce")]
    public async Task<IActionResult> ReduceStock([FromBody] StockReductionRequest request)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductName.ToLower() == request.ProductName.ToLower());

        if (item == null)
        {
            // Ako proizvod ne postoji, kreiraj ga (za potrebe demonstracije)
            item = new InventoryItem { ProductName = request.ProductName, QuantityInStock = 100 };
            _context.InventoryItems.Add(item);
        }

        if (item.QuantityInStock < request.Quantity)
        {
            return BadRequest("Nema dovoljno proizvoda na zalihi.");
        }

        item.QuantityInStock -= request.Quantity;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
