using OrderManagement.Application.DTOs.ProductDTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.Interface.ServiceContracts;

public interface IProductService
{
    public Task<List<ProductResponse>> GetAllProductsAsync();

    public Task<ProductResponse?> GetProductByIDAsync(Guid productId);

    public Task<ProductResponse> CreateProductAsync(ProductRequest product);
    public Task<bool> DeleteProductAsync(Guid productId);
    public Task<ProductResponse> UpdateProductAsync(Guid productId, ProductRequest product);

    public  Task<ProductResponse> UpdateProductPriceAsync(Guid productId, UpdateProductPriceRequest request);

    public Task<Product?> CheckForDuplicateProductByNameAsync(string name);

}
