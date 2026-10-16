
using OrderManagement.Domian.Entities;

namespace OrderManagement.Application.DTOs;

public class AuthenticationResponse
{
    public Guid UserID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    //extra 
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }
}

public static class AuthenticationResponseExtensions
{
    public static AuthenticationResponse ToAuthenticationResponse(this User user)
    {
        return new AuthenticationResponse
        {
            Email = user.Email,
            UserID = user.UserID,
            Password = user.PasswordHash,
            UserName = user.UserName,
            Role = user.Role,
            Success = false,
            Token = "token",
            Expiration = DateTime.UtcNow
        };
    }
}
