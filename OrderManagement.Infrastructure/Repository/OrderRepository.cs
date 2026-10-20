using Dapper;
using OrderManagement.Application.DTOs.CreateOrderDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Domian.Entities;
using OrderManagement.Infrastructure.ConnectionFactory;
using System.Data;

namespace OrderManagement.Infrastructure.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    public OrderRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateOrderRepo(Order order, IEnumerable<OrderItem> items)
    {
        var orderItems = items.ToList();
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

       using var transaction = await connection.BeginTransactionAsync(); 
        try
        {
         // 1. Execute CreateOrder stored procedure.
            await connection.ExecuteAsync(
              "CreateOrder",
               new
               {
                   order.OrderId,
                   UserID = order.UserID,
                   order.Status,
                   order.TotalAmount,
                   order.ShippingAddress,
                   order.ExpectedDeliveryDate,
                   order.CreatedAt,
                   order.UpdatedAt
               },
               transaction: transaction,
               commandType: CommandType.StoredProcedure
             );

            // 2. Execute CreateOrderItem stored procedure for each item.
            foreach (var item in orderItems)
            {
                await connection.ExecuteAsync(
                    "dbo.CreateOrderItem",
                    new
                    {
                        item.OrderItemId,
                        order.OrderId,
                        item.ProductId,
                        item.Quantity,
                        item.UnitPrice
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );
            }

            // 3. Reduce product stock using stored procedures.
            foreach (var item in orderItems)
            {
                await connection.ExecuteAsync(
                      "dbo.ReduceProductStock",
                      new {
                      item.ProductId,
                      item.Quantity,
                      UpdatedAt = DateTime.UtcNow
                      },
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
            }



            //if all methods is successed Save Permanently in DB
            await transaction.CommitAsync();

            return order.OrderId;
        }
        catch
        {
            //if anyone method service fails rollback
           await transaction.RollbackAsync();
           throw;
        }
    }

    public async Task<List<Order>> GetAllOrdersRepo()
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        // Query 1: Retrieve orders.
        var orders = (await connection.QueryAsync<Order>(
            "GetAllOrders",
            commandType: CommandType.StoredProcedure)).ToList();

        if(orders.Count == 0)
        {
            return orders;
        }

        // Query 2: Retrieve all order items.
        var orderItems = (await connection.QueryAsync<OrderItem>
            ("dbo.GetAllOrderItems",
              commandType: CommandType.StoredProcedure)).ToList();

        // Attach each item to its corresponding order.
        foreach(var order in orders)
        {
            order.OrderItems = orderItems
                .Where(item => item.OrderId == order.OrderId)
                .ToList();
        }

        return orders;
    }

    public async Task<bool> UpdateOrderStatusAndDateRepo(UpdateOrder updateOrder , Guid orderID)
    {
        using var connection = _connectionFactory.CreateConnection();

        var rowAffected =  await connection.ExecuteAsync("UpdateOrderStatus",
            new { 
                  OrderID = orderID ,
                  updateOrder.ExpectedDeliveryDate,
                  updateOrder.Status},
                  commandType: CommandType.StoredProcedure
            );

        return rowAffected > 0;
    }
}
