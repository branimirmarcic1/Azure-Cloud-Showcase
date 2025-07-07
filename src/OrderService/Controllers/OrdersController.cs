using System.Net.Http.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using Contracts; // <-- DODAJTE OVAJ USING

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpClientFactory _httpClientFactory;

    public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        var inventoryClient = _httpClientFactory.CreateClient("InventoryClient");

        try
        {
            // ISPRAVAK: Koristimo snažno tipizirani objekt iz Contracts projekta
            var requestBody = new StockReductionRequest
            {
                ProductName = createOrderDto.ProductName,
                Quantity = createOrderDto.Quantity
            };
            var response = await inventoryClient.PostAsJsonAsync("/api/inventory/reduce", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest($"Greška u servisu za zalihe: {error}");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Nije moguće kontaktirati servis za zalihe: {ex.Message}");
        }

        var order = new Order
        {
            ProductName = createOrderDto.ProductName,
            Quantity = createOrderDto.Quantity,
            TotalPrice = createOrderDto.Price * createOrderDto.Quantity
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new OrderCreated(order.Id, order.ProductName, order.Quantity));

        return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
    }
}