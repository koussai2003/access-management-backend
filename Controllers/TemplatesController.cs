using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using AccessManagementAPI.Services;

namespace AccessManagementAPI.Controllers
{
[ApiController]
[Route("api/templates")]
public class TemplatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TemplatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    {
        var templates = await _context.RequestTemplate.ToListAsync();
        return Ok(templates);
    }
    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest request)
    {
        var template = new RequestTemplate
    {
        Name = request.Name,
        Description = request.Description,
        ApplicationName = request.ApplicationName,
        ModulesJson = request.ModulesJson,
        CreatedBy = User.Identity?.Name ?? "System", // Set on server
        CreatedAt = DateTime.UtcNow // Also set timestamp on server
    };

    _context.RequestTemplate.Add(template);
    await _context.SaveChangesAsync();
    return Ok(template);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var template = await _context.RequestTemplate.FindAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        template.Name = request.Name;
        template.Description = request.Description;
        template.ApplicationName = request.ApplicationName;
        template.ModulesJson = request.ModulesJson;
        template.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(template);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TemplateExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
    }
    }
    private bool TemplateExists(int id)
    {
        return _context.RequestTemplate.Any(e => e.Id == id);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplate(int id)
    {
        var template = await _context.RequestTemplate.FindAsync(id);
        if (template == null) return NotFound();
        return Ok(template);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        var template = await _context.RequestTemplate.FindAsync(id);
        if (template == null) return NotFound();
        
        _context.RequestTemplate.Remove(template);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
}