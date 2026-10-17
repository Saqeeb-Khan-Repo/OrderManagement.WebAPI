using OrderManagement.Domian.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs.ProductDTOs;

/// <summary>
/// Recieve Product request and convert to Product Entity
/// </summary>
public class ProductRequest
{
    [Required(ErrorMessage = "Product name is Required")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = " Price is required")]
    [Range(99 , 9999999 )]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(1,1000)]
    public int StockQuantity { get; set; }

    //convert method
    public Product ToProduct()
    {
        return new Product
        {
            Name = Name,
            Description = Description,
            Price = Price,
            StockQuantity = StockQuantity
        };
    }
}
