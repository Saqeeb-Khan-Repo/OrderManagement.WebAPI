using OrderManagement.Application.DTOs.CreateOrderDTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.Interface.ServiceContracts;

public interface IOrderService
{
    public Task<Guid> CreateOrderAsync(Guid userID,Order order, IEnumerable<OrderItem> items);

    public Task<List<Order>> GetAllOrdersAsync();

    public Task<bool> UpdateOrderStatusAndDateAsync(UpdateOrder order, Guid orderID);
}
