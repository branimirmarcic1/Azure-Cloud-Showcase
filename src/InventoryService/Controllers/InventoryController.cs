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
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(InventoryDbContext context, ILogger<InventoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

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

    [HttpPost("reduce")]
    public async Task<IActionResult> ReduceStock([FromBody] StockReductionRequest request)
    {
        _logger.LogInformation("--> Primljen zahtjev za smanjenje zaliha za proizvod: {ProductName}, Količina: {Quantity}", request?.ProductName, request?.Quantity);

        if (!ModelState.IsValid)
        {
            _logger.LogError("--> Model state nije ispravan.");
            return BadRequest(ModelState);
        }

        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductName.ToLower() == request.ProductName.ToLower());

        if (item == null)
        {
            _logger.LogInformation("--> Proizvod {ProductName} nije pronađen. Kreiram novi.", request.ProductName);
            item = new InventoryItem { ProductName = request.ProductName, QuantityInStock = 100 };
            _context.InventoryItems.Add(item);
        }

        if (item.QuantityInStock < request.Quantity)
        {
            _logger.LogWarning("--> Nema dovoljno zaliha za {ProductName}. Potrebno: {Required}, Dostupno: {Available}", request.ProductName, request.Quantity, item.QuantityInStock);
            return BadRequest("Nema dovoljno proizvoda na zalihi.");
        }

        item.QuantityInStock -= request.Quantity;
        _logger.LogInformation("--> Smanjujem zalihe za {ProductName}. Nova količina: {NewQuantity}", request.ProductName, item.QuantityInStock);
        await _context.SaveChangesAsync();

        return Ok();
    }
}