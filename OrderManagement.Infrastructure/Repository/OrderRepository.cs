

using Dapper;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Domian.Entities;
using OrderManagement.Infrastructure.ConnectionFactory;
using System.Data;

namespace OrderManagement.Infrastructure.Repository;

public class OrderRepository : IOrderRepo
{
    private readonly SqlConnectionFactory _connectionFactory;
    public OrderRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateOrder(Order order, IDbTransaction transaction)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "CreateOrder",
            order,
            transaction: transaction,
            commandType: CommandType.StoredProcedure
            );

        return order.OrderId;
    }

    public Task CreateOrderItemsAsync(IEnumerable<OrderItem> items, IDbTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
