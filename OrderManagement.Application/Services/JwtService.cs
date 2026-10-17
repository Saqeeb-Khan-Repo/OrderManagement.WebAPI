
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application.DTOs.AuthDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Domian.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagement.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AuthenticationResponse> CreateJwtToken(User user)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(
                _configuration["Jwt:Expiration_Minute"]
            )
        );

    
        Claim[] claims =
        [
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.UserID.ToString()
            ),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            ),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserID.ToString()
            ),
            new Claim(
                ClaimTypes.Email,
                user.Email
            ),
            new Claim(
                ClaimTypes.Role , 
                user.Role
            )
        ];

        string key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT signing key is missing."
            );

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        string encodedToken =
            new JwtSecurityTokenHandler().WriteToken(token);

        var response = user.ToAuthenticationResponse();
        response.Token = encodedToken;
        response.Expiration = expiration;
        response.Success = true;

        return Task.FromResult(response);
    }
}
