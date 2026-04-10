namespace SimpelTransport.CodeTest.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string Status { get; set; } = "Pending";
    public string Details { get; set; } = "";
    public decimal Price { get; set; } = 100m;
}