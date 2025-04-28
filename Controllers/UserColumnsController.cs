using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Data;
using AccessManagementAPI.Models;

namespace AccessManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserColumnsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserColumnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserColumns/fonctions
        [HttpGet("{columnType}")]
        public async Task<IActionResult> GetColumnValues(string columnType)
        {
            if (!IsValidColumnType(columnType))
                return BadRequest("Invalid column type");

            var values = await _context.UserColumns
                .Where(uc => uc.ColumnType.ToLower() == columnType.ToLower())
                .Select(uc => uc.Value)
                .ToListAsync();

            return Ok(values);
        }

        // POST: api/UserColumns
        [HttpPost]
        public async Task<IActionResult> AddColumnValue([FromBody] UserColumnInputModel input)
        {
            if (!IsValidColumnType(input.ColumnType))
                return BadRequest("Invalid column type");

            if (await _context.UserColumns.AnyAsync(uc => 
                uc.ColumnType.ToLower() == input.ColumnType.ToLower() && 
                uc.Value.ToLower() == input.Value.ToLower()))
            {
                return BadRequest("Value already exists for this column type");
            }

            var userColumn = new UserColumn
            {
                ColumnType = input.ColumnType,
                Value = input.Value
            };

            _context.UserColumns.Add(userColumn);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Column value added successfully!" });
        }

        // DELETE: api/UserColumns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumnValue(int id)
        {
            var userColumn = await _context.UserColumns.FindAsync(id);
            if (userColumn == null)
                return NotFound();

            // Check if any users are using this value
            bool isUsed = false;
            switch (userColumn.ColumnType.ToLower())
            {
                case "fonction":
                    isUsed = await _context.Users.AnyAsync(u => u.Fonction == userColumn.Value);
                    break;
                case "societe":
                    isUsed = await _context.Users.AnyAsync(u => u.Societe == userColumn.Value);
                    break;
                case "direction":
                    isUsed = await _context.Users.AnyAsync(u => u.Direction == userColumn.Value);
                    break;
            }

            if (isUsed)
                return BadRequest("Cannot delete this value as it is currently in use by one or more users");

            _context.UserColumns.Remove(userColumn);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Column value deleted successfully" });
        }

        private bool IsValidColumnType(string columnType)
        {
            return columnType.ToLower() switch
            {
                "fonction" or "societe" or "direction" => true,
                _ => false
            };
        }
    }
}