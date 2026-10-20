
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs.CreateOrderDTOs;

public sealed class CreateOrderRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
    [MaxLength(100, ErrorMessage = "An order cannot contain more than 100 items.")]
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
