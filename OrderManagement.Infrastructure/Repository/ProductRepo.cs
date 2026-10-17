
using Dapper;
using OrderManagement.Domian.Entities;
using OrderManagement.Infrastructure.ConnectionFactory;
using OrderManagement.Application.DTOs.ProductDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;

namespace OrderManagement.Infrastructure.Repository;

public class ProductRepo : IProductRepo
{
    private readonly SqlConnectionFactory _ConnectionFactory;
    public ProductRepo(SqlConnectionFactory sqlConnection)
    {
        _ConnectionFactory = sqlConnection;
    }

    public async Task<List<Product>> GetAllProductsRepo()
    {
      using  var connection = _ConnectionFactory.CreateConnection();

       IEnumerable<Product> products  = await connection.QueryAsync<Product>(
            "sp_GetAllProducts",
            commandType: System.Data.CommandType.StoredProcedure
            );

        return products.ToList();
    }

    public async Task<Product?> GetProductByIDRepo(Guid productId)
    {
        using var connection = _ConnectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Product>(
              "sp_GetProductByID",
              new {ProductID = productId},
              commandType: System.Data.CommandType.StoredProcedure
              );
    }
    public async Task<Product> CreateProductRepo(Product product)
    {
        using var connection = _ConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "CreateProduct",
            new {
                product.ProductId,
                product.Price,
                product.Description,
                product.Name,
                product.StockQuantity,
                product.CreatedAt,
                product.UpdatedAt
                }
            );

        return product;
    }

    public async Task<bool> DeleteProductRepo(Guid productId)
    {
        using var connection = _ConnectionFactory.CreateConnection();
        var rowsEffected =  await connection.QuerySingleOrDefaultAsync<int>(
            "DeleteProductByID",
            new {  ProductID = productId},
            commandType: System.Data.CommandType.StoredProcedure);


        return rowsEffected > 0;
    }

   

    public async Task<Product> UpdateProductRepo(Guid productId, Product product)
    {
        using var connection = _ConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UpdateProductByID",
            new {ProductID = productId,
                 product.Name,
                 product.Description,
                 product.Price,
                 product.StockQuantity,
                 product.CreatedAt,
                 product.UpdatedAt,
            }
            );

        return product;
    }

    public async Task<Product> UpdateProductPriceRepo(Guid productId, UpdateProductPriceRequest request)
    {
         var existingProduct = await GetProductByIDRepo(productId);

       using var connection = _ConnectionFactory.CreateConnection(); //Using: Automatically closes Db connection when task is over

       var product = await connection.ExecuteAsync(
            "UpdateProductPrice",
            new { productId, request.Price },
            commandType:System.Data.CommandType.StoredProcedure
            );

        if(existingProduct == null)
        {
            throw new ArgumentNullException("Product is empty");
        }

        return existingProduct;
    }

    public async Task<Product?> CheckForDuplicateProductByNameRepo(string name)
    {
        using var connection = _ConnectionFactory.CreateConnection();

        Product? product = await connection.QueryFirstOrDefaultAsync<Product>("CheckForDuplicateProductByName",
            new {Name = name},
            commandType: System.Data.CommandType.StoredProcedure
            );

        return product;
    }
}
