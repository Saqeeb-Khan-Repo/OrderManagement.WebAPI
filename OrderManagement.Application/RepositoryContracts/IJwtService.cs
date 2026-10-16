using OrderManagement.Application.DTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.RepositoryContracts;

public interface IJwtService
{
    public Task<AuthenticationResponse> CreateJwtToken(User user);
}
