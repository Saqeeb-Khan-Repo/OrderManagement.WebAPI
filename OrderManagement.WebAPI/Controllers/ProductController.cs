using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.ProductDTOs;
using OrderManagement.Application.Interface.ServiceContracts;

namespace OrderManagement.WebAPI.Controllers;

public class ProductController : CustomController
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetProductByID(Guid productId)
    {
        var product = await _productService.GetProductByIDAsync(productId);

        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();

        return Ok(products);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{productId:guid}/price")]
    public async Task<IActionResult> UpdateProductPrice(Guid productId, UpdateProductPriceRequest request)
    {
        //check if the product exist or not
        var product = await _productService.GetProductByIDAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        if (request.Price <= 0)
        {
            return BadRequest(new { message = "Price must be greater than zero" });
        }

        //updating the price
        await _productService.UpdateProductPriceAsync(productId, request);
        return Content("Updated Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductRequest product)
    {
        if (product == null)
        {
            return BadRequest(new { message = "Invalid product details" });
        }

        var existing = await _productService.CheckForDuplicateProductByNameAsync(product.Name);
        if (existing != null)
        {
            return BadRequest(new
            {
                message = "Product Already Exists"
            });
        }

        var created = await _productService.CreateProductAsync(product);

        return Ok(created);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{productID:guid}")]
    public async Task<IActionResult> DeleteProductByID(Guid productID)
    {
        if( productID == Guid.Empty)
        {
            return BadRequest(new
            { 
                message = "ProductID is null or invalid" 
            });
        }
       var deleted = await _productService.DeleteProductAsync(productID);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }
        return Content("Product Deleted SuccessFully");
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{productID}")]
    public async Task<IActionResult> UpdateProductByID([FromBody] ProductRequest product , Guid productID)
    {
       var updated = await _productService.UpdateProductAsync(productID, product);

        if(updated == null)
        {
            return NotFound(new {error = "product not found" });
        }

        return Ok(updated);
    }


    }
