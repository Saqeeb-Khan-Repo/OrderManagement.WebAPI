
using OrderManagement.Application.ServiceContracts;
using OrderManagement.Domian.Entities;
using OrderManagement.Application.RepositoryContracts;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _productRepository;

    public ProductService(IProductRepo productRepo)
    {
        _productRepository = productRepo;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
      return  await _productRepository.GetAllProductsRepo();
    }

    public async Task<Product?> GetProductByIDAsync(Guid productId)
    {
        var product = await _productRepository.GetProductByIDRepo(productId);

        if(product == null)
        {
            throw new ArgumentNullException("ProductID is invalid or No Product exist with id");
        }
        return product;
    }
    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.ProductId = Guid.NewGuid();
        product.UpdatedAt = null;

        var newCreatedProduct = await _productRepository.CreateProductRepo(product);

        if(newCreatedProduct == null)
        {
            throw new ArgumentNullException("Invalid Product details");
        }

        return product;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        return await _productRepository.DeleteProductRepo(productId);
    }

   

    public async Task<Product> UpdateProductAsync(Guid productId, Product product)
    {
        //getByID and assign created at value from previous
        var existing =  await _productRepository.GetProductByIDRepo(productId);

        //generate time for update
        product.UpdatedAt = DateTime.UtcNow;
        product.CreatedAt = existing.CreatedAt;
        var updated = await _productRepository.UpdateProductRepo(productId, product);

        if(updated == null)
        {
            throw new ArgumentNullException("Product not found");
        }

        return updated;
    }

    public async Task<Product> UpdateProductPriceAsync(Guid productId, UpdateProductPriceRequest request)
    {
        return await _productRepository.UpdateProductPriceRepo(productId, request);
    }

    public async Task<Product?> CheckForDuplicateProductByNameAsync(string name)
    {
        return await _productRepository.CheckForDuplicateProductByNameRepo(name);
    }
}
