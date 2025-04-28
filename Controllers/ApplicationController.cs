using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Data;
using AccessManagementAPI.Models;

namespace AccessManagementAPI.Controllers
{
    [Route("api/applications")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Get all applications
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            var applications = await _context.Applications.ToListAsync();
            return Ok(applications);
        }

        // ✅ Add a new application
        [HttpPost]
        public async Task<IActionResult> AddApplication([FromBody] Application newApp)
        {
            _context.Applications.Add(newApp);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Application added successfully!" });
        }
        [HttpPut("{id}")]
public async Task<IActionResult> UpdateApplication(int id, [FromForm] IFormFile? logo, [FromForm] string name)
{
    var app = await _context.Applications.FindAsync(id);
    if (app == null) return NotFound(new { message = "Application not found" });

    app.Name = name; 

    if (logo != null && logo.Length > 0)  
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, logo.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await logo.CopyToAsync(stream);
        }

        app.LogoUrl = $"/uploads/{logo.FileName}"; 
    }

    await _context.SaveChangesAsync();
    return Ok(new { message = "Application updated successfully!" });
}


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
        var app = await _context.Applications.FindAsync(id);
        if (app == null) return NotFound(new { message = "Application not found" });

        _context.Applications.Remove(app);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Application deleted successfully!" });
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadApplication([FromForm] IFormFile logo, [FromForm] string name)
        {
        if (logo == null || logo.Length == 0)
        return BadRequest(new { message = "No file uploaded" });

        // ✅ Save file to "wwwroot/uploads" directory
        if (string.IsNullOrEmpty(name))
        return BadRequest(new { message = "Application name is required" }); // ✅ Log issue

        try
        {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, logo.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
        await logo.CopyToAsync(stream);
        }

        var newApp = new Application
        {
            Name = name,
            LogoUrl = $"{Request.Scheme}://{Request.Host}/uploads/{logo.FileName}" // ✅ Save relative URL to database
        };

        _context.Applications.Add(newApp);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Application added successfully!" });
        }
         catch (Exception ex)
    {
        return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
    }
    }
        }
}
