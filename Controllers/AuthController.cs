using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using BCrypt.Net;

namespace AccessManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ REGISTER USER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                return BadRequest(new { message = "Email already exists" });

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = userDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { message = "Login successful", userId = user.Id,email = user.Email, name = user.Name,role = user.Role,mustChangePassword = user.MustChangePassword,validateur1 = user.Validateur1,validateur2 = user.Validateur2,validateur3 = user.Validateur3,societe = user.Societe,fonction = user.Fonction, direction = user.Direction});
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
        {
        return NotFound(new { message = "User not found" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User deleted successfully" });
    }
        [HttpGet("verify/{id}")]
        public async Task<IActionResult> VerifyUserExists(int id)
    {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
             return Unauthorized(new { message = "User deleted" });

            return Ok(new { message = "User still valid" });
    }   

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var user = await _context.Users.FindAsync(changePasswordDto.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
        user.MustChangePassword = false; // ✅ Mark as password changed

        await _context.SaveChangesAsync();
        return Ok(new { message = "Password changed successfully" });
}

    }

    // ✅ DTOs (Data Transfer Objects) for Security
    public class UserRegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }  // Plain password
    }
    public class ChangePasswordDTO
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
