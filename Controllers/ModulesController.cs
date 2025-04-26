using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using AccessManagementAPI.Dtos;

namespace AccessManagementAPI.Controllers
{
    [ApiController]
    [Route("api/modules")]
    public class ModulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ POST: Save modules for a specific application
        [HttpPost("{applicationId}")]
        public async Task<IActionResult> SaveModules(int applicationId, [FromBody] List<ModuleDto> modules)
        {
            var app = await _context.Applications
                .Include(a => a.Modules)
                .ThenInclude(m => m.Functions)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null)
                return NotFound("Application not found");

            // Remove old modules (and their functions)
            _context.Modules.RemoveRange(app.Modules);
            await _context.SaveChangesAsync();

            foreach (var modDto in modules)
            {
                var newModule = new Module
                {
                    Name = modDto.Name,
                    ApplicationId = applicationId,
                    Functions = modDto.Functions.Select(f => new ModuleFunction
                    {
                        Name = f.Name,
                        Comment = f.Comment,
                        Consultation = f.Options.Consultation,
                        Creation = f.Options.Creation,
                        Modification = f.Options.Modification,
                        Suppression = f.Options.Suppression,
                        Validation = f.Options.Validation
                    }).ToList()
                };

                _context.Modules.Add(newModule);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Modules saved successfully" });
        }

        // ✅ GET: Retrieve modules + functions for an app
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetModules(int applicationId)
        {
            var modules = await _context.Modules
                .Include(m => m.Functions)
                .Where(m => m.ApplicationId == applicationId)
                .ToListAsync();

            var result = modules.Select(m => new ModuleDto
    {
                Name = m.Name,
                Functions = m.Functions.Select(f => new FunctionDto
            {
                    Name = f.Name,
                    Comment = f.Comment,
                    Options = new FunctionOptionsDto
            {
                        Consultation = f.Consultation,
                        Creation = f.Creation,
                        Modification = f.Modification,
                        Suppression = f.Suppression,
                        Validation = f.Validation
            }
        }).ToList()
    });

    return Ok(result);
        }
    }
}
