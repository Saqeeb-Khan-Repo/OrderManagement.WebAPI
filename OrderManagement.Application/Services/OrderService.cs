using OrderManagement.Application.DTOs.CreateOrderDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Application.Interface.ServiceContracts;
using OrderManagement.Domian.Entities;



namespace OrderManagement.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepo _productRepo;

    public OrderService(IOrderRepository orderRepository , IProductRepo productRepo)
    {
        _orderRepository = orderRepository;
        _productRepo = productRepo;
    }
    public async Task<Guid> CreateOrderAsync(Guid userID ,Order order, IEnumerable<OrderItem> items)
    {
        var requestedItems = items.ToList();
        //validate
        if(requestedItems.Count <= 0)
        {
            throw new ArgumentException("Order must be Greater than zero");
        }
        if(requestedItems.Any(e => e.Quantity <= 0))
        {
            throw new ArgumentException("Quantity must be Greater than zero");
        }
        if (requestedItems.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
            throw new ArgumentException("Duplicate products are not allowed.");


        order.OrderId = Guid.NewGuid();
        order.UserID = userID;
        order.Status = "Pending";
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = null;

        decimal totalPrice = 0;

        //OrderItems 
        var orderItemsToSave = new List<OrderItem>();
        foreach (var item in requestedItems)
        {

            // Get product details from the database.
            var product = await _productRepo
                .GetProductByIDRepo(item.ProductId);

            if (product is null)
                throw new InvalidOperationException(
                    $"Product {item.ProductId} was not found.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}.");

            //total price calculation
            totalPrice += product.Price * item.Quantity;

            // Prepare the order item for insertion.
            orderItemsToSave.Add(new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }
        order.TotalAmount = totalPrice;

        return await _orderRepository.CreateOrderRepo(order, orderItemsToSave);

    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var listOfOrders = await _orderRepository.GetAllOrdersRepo();

        return listOfOrders;
    }

    public async Task<bool> UpdateOrderStatusAndDateAsync(UpdateOrder updateOrder, Guid orderID)
    {
        if(orderID == Guid.Empty)
        {
            throw new ArgumentException("orderID is empty");
        }

        return await _orderRepository.UpdateOrderStatusAndDateRepo(updateOrder, orderID);
    }
}
