using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.CreateOrderDTOs;
using OrderManagement.Application.Interface.ServiceContracts;
using OrderManagement.Domian.Entities;
using System.Security.Claims;

namespace OrderManagement.WebAPI.Controllers;

public class OrderController : CustomController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService service)
    {
        _orderService = service;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
    CreateOrderRequest request)
    {
        try { 
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var order = new Order
        {
            ShippingAddress = request.ShippingAddress
        };

        var items = request.Items.Select(item => new OrderItem
        {
            ProductId = item.ProductID,
            Quantity = item.Quantity
        }).ToList();

        var orderId = await _orderService.CreateOrderAsync(
            userId,
            order,
            items);

        return StatusCode(
            StatusCodes.Status201Created,
            new { orderId, message = "Order created successfully." });
    } catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
            var listOfOrders = await _orderService.GetAllOrdersAsync();

            return Ok(listOfOrders);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{orderID:guid}/status")]
    public async Task<IActionResult> UpdateStatusAndExpectedDate(Guid orderID , UpdateOrder updateOrder)
    {
       var updated =  await _orderService.UpdateOrderStatusAndDateAsync(updateOrder, orderID);
        if (!updated)
        {
            return BadRequest(new {message = "Already updated fields" });
        }
        return Ok(new{message = "Updated Successfully "});
    }
}
