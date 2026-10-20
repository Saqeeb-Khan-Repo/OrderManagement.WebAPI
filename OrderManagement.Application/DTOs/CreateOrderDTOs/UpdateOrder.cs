

namespace OrderManagement.Application.DTOs.CreateOrderDTOs;

public class UpdateOrder
{
    public string Status { get; set; } = string.Empty;

    public DateTime? ExpectedDeliveryDate { get; set; }
}
