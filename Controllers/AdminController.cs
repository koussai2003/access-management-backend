using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Data;
using AccessManagementAPI.Models;


namespace AccessManagementAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Get all users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] UserInputModel newUser)
        {
            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
                return BadRequest(new { message = "Email already exists" });
            if (string.IsNullOrEmpty(newUser.Password))
                return BadRequest(new { message = "Password is required." });
            var validateur1Exists = await _context.Users.AnyAsync(u => u.Email == newUser.Validateur1);
            if (!validateur1Exists)
                return BadRequest(new { message = "Validateur 1 email does not match any user." });
            if (!string.IsNullOrEmpty(newUser.Validateur2))
            {
                var validateur2Exists = await _context.Users.AnyAsync(u => u.Email == newUser.Validateur2);
                if (!validateur2Exists)
                    return BadRequest(new { message = "Validateur 2 email does not match any user." });
            }

            if (!string.IsNullOrEmpty(newUser.Validateur3))
            {
                    var validateur3Exists = await _context.Users.AnyAsync(u => u.Email == newUser.Validateur3);
                    if (!validateur3Exists)
                        return BadRequest(new { message = "Validateur 3 email does not match any user." });
            }
            var user = new User
            {
                Name = newUser.Name,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Role = newUser.Role,
                Fonction = newUser.Fonction,
                Societe = newUser.Societe,
                Direction = newUser.Direction,
                Statut = newUser.Statut,
                Validateur1 = newUser.Validateur1,
                Validateur2 = newUser.Validateur2,
                Validateur3 = newUser.Validateur3
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully!" });
}


        // ✅ Delete user
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            var validateur1Exists = await _context.Users.AnyAsync(u => u.Email == updatedUser.Validateur1);
            if (!validateur1Exists)
                return BadRequest(new { message = "Validateur 1 email does not match any user." });
            if (!string.IsNullOrEmpty(updatedUser.Validateur2))
            {
                var validateur2Exists = await _context.Users.AnyAsync(u => u.Email == updatedUser.Validateur2);
                if (!validateur2Exists)
                    return BadRequest(new { message = "Validateur 2 email does not match any user." });
            }
            if (!string.IsNullOrEmpty(updatedUser.Validateur3))
            {
                var validateur3Exists = await _context.Users.AnyAsync(u => u.Email == updatedUser.Validateur3);
                if (!validateur3Exists)
                    return BadRequest(new { message = "Validateur 3 email does not match any user." });
            }
            user.Name = updatedUser.Name;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Fonction = updatedUser.Fonction;
            user.Societe = updatedUser.Societe;
            user.Direction = updatedUser.Direction;
            user.Statut = updatedUser.Statut;
            user.Validateur1 = updatedUser.Validateur1;
            user.Validateur2 = updatedUser.Validateur2;
            user.Validateur3 = updatedUser.Validateur3;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully!" });
}

    }
    public class UserUpdateDTO
    {
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; } // ✅ Optional password update
    }
}
