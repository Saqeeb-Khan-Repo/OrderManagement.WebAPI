
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs.CreateOrderDTOs;

public sealed class CreateOrderRequest
{
    [Required]
    [Range(1,100,ErrorMessage = "Orders must between 1 to 100")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];

    [Required]
    [StringLength(500, MinimumLength = 5)]
    public string ShippingAddress { get; set; } = string.Empty;
}
public sealed class CreateOrderItemRequest
{
   public Guid ProductID { get; set; }

    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; } 
}
