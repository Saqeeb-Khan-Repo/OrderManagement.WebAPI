using OrderManagement.Domian.Entities;
using OrderManagement.Application.DTOs.ProductDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Application.Interface.ServiceContracts;

namespace OrderManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _productRepository;

    public ProductService(IProductRepo productRepo)
    {
        _productRepository = productRepo;
    }

    public async Task<List<ProductResponse>> GetAllProductsAsync()
    {
      var productsList =  await _productRepository.GetAllProductsRepo();
        return productsList.Select(p => p.ToProductResponse()).ToList();
    }

    public async Task<ProductResponse?> GetProductByIDAsync(Guid productId)
    {
        var product = await _productRepository.GetProductByIDRepo(productId);

        if(product == null)
        {
            throw new ArgumentNullException("ProductID is invalid or No Product exist with id");
        }
        return product.ToProductResponse();
    }
    public async Task<ProductResponse> CreateProductAsync(ProductRequest product)
    {
        //ProductRequest => Product
        var productModel = product.ToProduct();

        productModel.CreatedAt = DateTime.UtcNow;
        productModel.ProductId = Guid.NewGuid();
        productModel.UpdatedAt = null;

        var newCreatedProduct = await _productRepository.CreateProductRepo(productModel);

        if(newCreatedProduct == null)
        {
            throw new ArgumentNullException("Invalid Product details");
        }

        //Product => ProductResponse
        return productModel.ToProductResponse();
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        return await _productRepository.DeleteProductRepo(productId);
    }

   

    public async Task<ProductResponse> UpdateProductAsync(Guid productId, ProductRequest product)
    {
        //getByID and assign created at value from previous
        var existing =  await _productRepository.GetProductByIDRepo(productId);

        //ProductRequest => Product
        var productModel = product.ToProduct();

        //generate time for update
        productModel.UpdatedAt = DateTime.UtcNow;
        productModel.CreatedAt = existing.CreatedAt;
        var updated = await _productRepository.UpdateProductRepo(productId, productModel);

        if(updated == null)
        {
            throw new ArgumentNullException("Product not found");
        }

        return updated.ToProductResponse();
    }

    public async Task<ProductResponse> UpdateProductPriceAsync(Guid productId, UpdateProductPriceRequest request)
    {

        var productModel = await _productRepository.UpdateProductPriceRepo(productId, request);

        return productModel.ToProductResponse();
    }

    public async Task<Product?> CheckForDuplicateProductByNameAsync(string name)
    {
        var productModel = await _productRepository.CheckForDuplicateProductByNameRepo(name);

        return productModel;
    }
}
