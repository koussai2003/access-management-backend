using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using AccessManagementAPI.Services;

namespace AccessManagementAPI.Controllers
{
    [ApiController]
    [Route("api/user/request")]
    public class UserRequestController : ControllerBase

    {
        private readonly ApplicationDbContext _context;

        public UserRequestController(ApplicationDbContext context)
    {
        _context = context;
    }

        [HttpPost]
        public async Task<IActionResult> SubmitRequest([FromBody] UserAccessRequest request)
    {
        if (request.IsOnBehalfRequest)
    {
        if (string.IsNullOrEmpty(request.ActualUserEmail))
        {
            return BadRequest("Actual user email is required for on-behalf requests");
        }

        
        var existingUser = await _context.Users.AnyAsync(u => u.Email == request.ActualUserEmail);
        if (existingUser)
        {
            return BadRequest("Cannot submit request for an existing user - they should submit their own request");
        }
    }
        request.State = "Not viewed";
        request.SubmittedAt = DateTime.Now;
        request.LockedByAdmin = null;
        request.ValidatorComment = null;
        request.RequestedByEmail = User.Identity?.Name;

        if (request.IsOnBehalfRequest)
    {
        request.UserEmail = request.ActualUserEmail;
        request.Societe = "To be determined";
        request.Fonction = "To be determined";
        request.Direction = "To be determined";
    }
    else
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.UserEmail);
        if (user != null)
        {
            request.Societe = user.Societe;
            request.Fonction = user.Fonction;
            request.Direction = user.Direction;
        }
    }
    
        _context.UserAccessRequests.Add(request);
        await _context.SaveChangesAsync();
        if (request.UserEmail == request.Validateur1)
        {
            request.ValidatedBy1 = true;

            if (!string.IsNullOrWhiteSpace(request.Validateur2))
            {
                request.State = "Pending Validateur 2";
            }
            else if (!string.IsNullOrWhiteSpace(request.Validateur3))
            {
                request.State = "Pending Validateur 3";
            }
            else
            {
                request.State = "Pending Admin";
            }

            _context.RequestValidations.Add(new RequestValidation
            {
                RequestId = request.Id,
                ValidatorEmail = request.UserEmail,
                ValidatedAt = DateTime.Now
            });
    }
        await _context.SaveChangesAsync();

        return Ok(new { message = "Request submitted" });
    }
    [HttpGet("history/{email}")]
    public async Task<IActionResult> GetUserRequestHistory(string email)
    {
        var history = await _context.UserAccessRequests
            .Where(r => r.UserEmail == email)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();

        return Ok(history);
    }
    [HttpGet("exists/{email}")]
    public async Task<IActionResult> CheckUserExists(string email)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        return Ok(exists);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequest(int id, [FromBody] UserAccessRequest updated)
    {
        var request = await _context.UserAccessRequests.FindAsync(id);
        if (request == null || request.State != "Not viewed") return BadRequest("Can't edit this request");

        request.ModulesJson = updated.ModulesJson;
        request.ApplicationName = updated.ApplicationName;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Request updated successfully" });
}

    [HttpGet("pending-for-validateur/{email}")]
    public async Task<IActionResult> GetRequestsForValidateur(string email)
{
    var requests = await _context.UserAccessRequests
        .Where(r =>
            ((r.Validateur1 == email && !r.ValidatedBy1) ||
            (r.Validateur2 == email && r.ValidatedBy1 && !r.ValidatedBy2) ||
            (r.Validateur3 == email && r.ValidatedBy1 && r.ValidatedBy2 && !r.ValidatedBy3)) &&
            !r.State.Contains("Rejected") // Exclude rejected requests
        )
        .OrderByDescending(r => r.SubmittedAt)
        .ToListAsync();

    return Ok(requests);
}
    [HttpGet("validations-history/{email}")]
    public async Task<IActionResult> GetValidationHistory(string email)
    {
    var validations = await _context.RequestValidations
        .Where(v => v.ValidatorEmail == email)
        .Include(v => v.Request)
        .OrderByDescending(v => v.ValidatedAt)
        .Take(30)
        .ToListAsync();

    return Ok(validations);
}
    [HttpPost("record-validation")]
    public async Task<IActionResult> RecordValidation([FromBody] ValidationRecordDto record)
    {
    var validation = new RequestValidation
    {
        RequestId = record.RequestId,
        ValidatorEmail = record.ValidatorEmail,
        ValidatedAt = DateTime.Parse(record.ValidatedAt),
        IsRejection = record.IsRejection
    };

    _context.RequestValidations.Add(validation);
    await _context.SaveChangesAsync();

    return Ok();
}


}
public class ValidationRecordDto
{
    public int RequestId { get; set; }
    public string ValidatorEmail { get; set; }
    public string ValidatedAt { get; set; }
    public bool IsRejection { get; set; }
}


}