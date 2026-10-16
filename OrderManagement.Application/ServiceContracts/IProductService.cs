

using OrderManagement.Application.DTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.ServiceContracts;

public interface IProductService
{
    public Task<List<Product>> GetAllProductsAsync();

    public Task<Product?> GetProductByIDAsync(Guid productId);

    public Task<Product> CreateProductAsync(Product product);
    public Task<bool> DeleteProductAsync(Guid productId);
    public Task<Product> UpdateProductAsync(Guid productId, Product product);

    public  Task<Product> UpdateProductPriceAsync(Guid productId, UpdateProductPriceRequest request);

    public Task<Product?> CheckForDuplicateProductByNameAsync(string name);

}
