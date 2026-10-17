using OrderManagement.Application.DTOs.AuthDTOs;
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.Interface.RepositoryContracts;

public interface IJwtService
{
    public Task<AuthenticationResponse> CreateJwtToken(User user);
}
