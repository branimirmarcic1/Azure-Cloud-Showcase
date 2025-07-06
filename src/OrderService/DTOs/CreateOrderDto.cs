namespace OrderService.DTOs;

public record CreateOrderDto(
    string ProductName,
    int Quantity,
    decimal Price
);