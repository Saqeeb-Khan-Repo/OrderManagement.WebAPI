

using OrderManagement.Domian.Entities;
using System.Data;

namespace OrderManagement.Application.Interface.RepositoryContracts;

public interface IOrderRepo
{
    /// <summary>
    /// Create an order method
    /// </summary>
    /// <param name="order"></param>
    /// <param name="transaction"></param>
    /// <returns>returns Order.OrderID </returns>
    public Task<Guid> CreateOrder(Order order , IDbTransaction transaction);

    public Task CreateOrderItemsAsync(
        IEnumerable<OrderItem> items,
        IDbTransaction transaction);
}
