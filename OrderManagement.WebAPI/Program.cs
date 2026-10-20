using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application;
using OrderManagement.Domian.Entities;
using OrderManagement.Infrastructure;
using OrderManagement.WebAPI.Middleware;
using System.Security.Claims;
using System.Text;


namespace OrderManagement.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                //adding policy for all controllers
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            });
            builder.Services.AddCore(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddOpenApi();
            //psw hash
            builder.Services.AddScoped<IPasswordHasher<User> , PasswordHasher<User>>();
           

            //adding schemes for auth + authorization
            builder.Services
             .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//scheme
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters =
                     new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidIssuer = builder.Configuration["Jwt:Issuer"],

                         ValidateAudience = true,
                         ValidAudience = builder.Configuration["Jwt:Audience"],

                         ValidateLifetime = true,

                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(
                                 builder.Configuration["Jwt:Key"]!
                             )
                         ),
                         RoleClaimType = ClaimTypes.Role
                     };
                 }
                 );

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //exception handling middleware globaly
            app.UseExceptionHandling();

            //https
            app.UseHttpsRedirection();

            //Authentication => Authorization => Controller
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
