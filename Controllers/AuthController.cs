using Azure.Core;
using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JC_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenRepository tokenRepository;

        public AuthController(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            this.userRepository = userRepository;
            this.tokenRepository = tokenRepository;
        }

        //Registering/Creating a user
        //POST: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var existingUser = await userRepository.GetByEmailAsync(registerRequestDTO.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email Already Exists!"
                });
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                FullName = registerRequestDTO.FullName,
                Email = registerRequestDTO.Email,
            };

            await userRepository.RegisterAsync(newUser, registerRequestDTO.Password, registerRequestDTO.Roles);

            return Ok(new
            {
                success = true,
                message = "Registration successful. Please login!"
            });

        }


        //Login a user
        //POST: api/Auth/Login
        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var user = await userRepository.GetByEmailAsync(loginRequestDTO.Email);

            if (user == null)
            {
                return BadRequest("User does not exist! Please sign up.");
            }

            // ✅ Check password before proceeding
            if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.PasswordHash))
            {
                return BadRequest("Username or Password Incorrect! Please Try Again.");
            }

            var roles = await userRepository.GetUserRolesAsync(user.UserId);

            var jwtToken = tokenRepository.CreateJWTToken(user, roles);

            var response = new LoginResponseDTO
            {
                JwtToken = jwtToken,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.ToArray()
            };

            return Ok(response);
        }


        //Generate Reset-Password Token
        //POST: api/Auth/forgot-password
        [HttpPost]
        [Route("forgot-password")]

        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            var success = await userRepository.GenerateResetTokenAsync(forgotPasswordRequestDTO.Email);

            if (!success)
            {
                return BadRequest("Email Not Found!");
            }

            Console.WriteLine(success);

            return Ok(new
            {
                success = true,
                message = "Reset token has been sent to your email."
            });

        }

        //Reset Password
        //POST: api/Auth/reset-password
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            var success = await userRepository.ResetPasswordAsync(resetPasswordRequestDTO.Email, resetPasswordRequestDTO.Token, resetPasswordRequestDTO.NewPassword);

            if (!success)
            {
                return BadRequest("Invalid Token or Token has Expired!");
            }

            Console.WriteLine(success);

            return Ok(new
            {
                success = true,
                message = "Password has been reset successfully. You can now log in."
            });

        }
    }
}
