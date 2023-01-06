namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class OrderDto
{
    public int Id { get; set; }
    public DateTime DateTimeCreated { get; set; }
    public string OrderNumber { get; set; }
    public double TotalPrice { get; set; }
    public int UserId { get; set; }
}