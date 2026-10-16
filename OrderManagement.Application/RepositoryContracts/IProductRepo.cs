using OrderManagement.Application.DTOs;
using OrderManagement.Domian.Entities;


namespace OrderManagement.Application.RepositoryContracts;
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
