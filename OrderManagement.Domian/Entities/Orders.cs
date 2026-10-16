

namespace OrderManagement.Domian.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Customer Customer { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
