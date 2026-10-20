

using OrderManagement.Application.DTOs.CreateOrderDTOs;
using OrderManagement.Domian.Entities;
using System.Data;

namespace OrderManagement.Application.Interface.RepositoryContracts;

public interface IOrderRepository
{
    /// <summary>
    /// Create an order method
    /// </summary>
    /// <param name="order"></param>
    /// <param name="transaction"></param>
    /// <returns>returns Order.OrderID </returns>
    public Task<Guid> CreateOrderRepo(Order order , IEnumerable<OrderItem> items);

    public Task<List<Order>> GetAllOrdersRepo();

    public Task<bool> UpdateOrderStatusAndDateRepo(UpdateOrder order, Guid orderID);
}
