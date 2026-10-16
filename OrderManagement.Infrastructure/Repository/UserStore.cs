using Dapper;
using OrderManagement.Application.RepositoryContracts;
using OrderManagement.Domian.Entities;
using OrderManagement.Infrastructure.ConnectionFactory;


namespace OrderManagement.Infrastructure.Repository;

public class UserStore : IUserStore
{

    private readonly SqlConnectionFactory _ConnectionFactory;
    public UserStore(SqlConnectionFactory sqlConnection)
    {
        _ConnectionFactory = sqlConnection;
    }

    public async Task Register(User user)
    {
        var connection = _ConnectionFactory.CreateConnection();

        //insert the user into db
        await connection.ExecuteAsync("AddNewUsers",
            new
            {
                user.UserID,
                user.Email,
                user.PasswordHash,
                user.Role,
                user.UserName
            },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<User?> CheckByEmailRepo(string email)
    {
        //CheckByEmail
        var connection = _ConnectionFactory.CreateConnection();
        var existingUser = await connection.QueryFirstOrDefaultAsync<User>
             ("GetByEmail",
             new { email },
             commandType: System.Data.CommandType.StoredProcedure
             );

        return existingUser;
    }

    public async Task<List<User>> GetAllUsersRepo()
    {
        var connection = _ConnectionFactory.CreateConnection();
        var listOfUsers = await connection.QueryAsync<User>(
             "GetAllUsers",
             commandType: System.Data.CommandType.StoredProcedure
             );

        return listOfUsers.ToList();
    }
}
