using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.DTOs.ProductDTOs;

public class ProductResponse
{
   public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
//convert Product into ProductResponse
public static class ProductExtensions
{
  public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse
        {
            Name = product.Name,
            Price = product.Price,
            ProductId = product.ProductId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Description = product.Description,
            CategoryId = product.CategoryId,
            StockQuantity = product.StockQuantity,
            OrderItems = product.OrderItems
        };
    }
}
