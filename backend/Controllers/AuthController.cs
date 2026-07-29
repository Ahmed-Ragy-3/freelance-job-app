using backend.DTOs;
using backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Services.JwtService _jwtService;

        public AuthController(AppDbContext context, Services.JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return BadRequest("Email is already registered.");

            if (dto.Role != "Freelancer" && dto.Role != "Client")
                return BadRequest("Role must be 'Freelancer' or 'Client'.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                Password = hashedPassword,
                Role = Enum.Parse<Role>(dto.Role)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (dto.Role == "Freelancer")
            {
                var freelancer = new Freelancer
                {
                    UserId = user.Id,
                    Bio = dto.Bio ?? ""
                };
                _context.Freelancers.Add(freelancer);
            }
            else
            {
                var client = new Client
                {
                    UserId = user.Id,
                    CompanyName = dto.CompanyName ?? ""
                };
                _context.Clients.Add(client);
            }

            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.UserName,
                Role = user.Role.ToString()
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!passwordValid)
                return Unauthorized("Invalid email or password.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.UserName,
                Role = user.Role.ToString()
            });
        }
    }
}