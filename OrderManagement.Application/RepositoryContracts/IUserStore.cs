using OrderManagement.Application.DTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.RepositoryContracts
{
    public interface IUserStore
    {
        public  Task Register(User user);
     
        public Task<User?> CheckByEmailRepo(string email);
        public  Task<List<User>> GetAllUsersRepo();
    }
}
