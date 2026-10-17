using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.AuthDTOs;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Domian.Entities;
namespace OrderManagement.WebAPI.Controllers
{
    [AllowAnonymous] //allow all to request
    public class AuthController : CustomController
    {
        //private filed
        private readonly IUserStore _userStore;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthController(IUserStore userStore, 
            IPasswordHasher<User> passwordHasher
            , IJwtService jwtService)
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
           var listOfUsers = await _userStore.GetAllUsersRepo();

            return Ok(listOfUsers);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var existingUser = await _userStore.CheckByEmailRepo(request.Email);

            if (existingUser != null) //user should be null or not existed before
            {
                return Conflict(new
                {
                    message = "An account with this email already exists."
                });
            }
            //convert request into user
                User user = new User
                {
                    UserID = Guid.NewGuid(),
                    Email = request.Email,
                    UserName = request.UserName,
                    Role = "Admin"
                };

                //password hashing
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

                //add user to db
                await _userStore.Register(user);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "User registered successfully."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var existingUser = await _userStore.CheckByEmailRepo(request.Email);

            // Use the same response for unknown emails and bad passwords.
            if (existingUser == null ||
                string.IsNullOrWhiteSpace(existingUser.PasswordHash))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            //verify passoword
            var result = _passwordHasher.VerifyHashedPassword(
                existingUser, 
                existingUser.PasswordHash,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            //generate jwt token from jwt service
            var response = await _jwtService.CreateJwtToken(existingUser);

            return Ok(response);
        }
    }
}
