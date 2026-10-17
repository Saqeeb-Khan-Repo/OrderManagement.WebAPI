using OrderManagement.Application.DTOs.ProductDTOs;
using OrderManagement.Domian.Entities;


namespace OrderManagement.Application.Interface.RepositoryContracts;
//UpdateProductPrice
public interface IProductRepo
{
    public Task<List<Product>> GetAllProductsRepo();

    public Task<Product?> GetProductByIDRepo(Guid productId);
   
    public Task<Product> CreateProductRepo(Product product); 
    public Task<bool> DeleteProductRepo(Guid productId); 
    public Task<Product> UpdateProductRepo(Guid productId , Product product);
    public Task<Product> UpdateProductPriceRepo(Guid productId, UpdateProductPriceRequest request);

    public Task<Product?> CheckForDuplicateProductByNameRepo(string name);

}
