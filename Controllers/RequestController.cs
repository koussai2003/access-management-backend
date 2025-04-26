using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using AccessManagementAPI.Services;

namespace AccessManagementAPI.Controllers
{
    [ApiController]
    [Route("api/admin/requests")]
    public class RequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly EmailTemplateService _templateService;
        public RequestController(ApplicationDbContext context, EmailService emailService,EmailTemplateService templateService)
        {
            _context = context;
            _emailService = emailService;
            _templateService = templateService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAccessRequest>>> GetAllRequests()
        {
            var requests = await _context.UserAccessRequests
                .Where(r =>!r.State.Contains("Rejected") &&
                    (string.IsNullOrWhiteSpace(r.Validateur1) && string.IsNullOrWhiteSpace(r.Validateur2) && string.IsNullOrWhiteSpace(r.Validateur3)) ||
                    (r.Validateur1 != null && r.ValidatedBy1 && string.IsNullOrWhiteSpace(r.Validateur2) && string.IsNullOrWhiteSpace(r.Validateur3)) ||
                    (r.Validateur2 != null && r.ValidatedBy1 && r.ValidatedBy2 && string.IsNullOrWhiteSpace(r.Validateur3)) ||
                    (r.Validateur3 != null && r.ValidatedBy1 && r.ValidatedBy2 && r.ValidatedBy3)
                )
                .OrderByDescending(r => r.SubmittedAt)
                .Take(30)
                .Select(r => new {
                    r.Id,
                    r.UserName,
                    r.UserEmail,
                    r.ApplicationName,
                    r.State,
                    r.SubmittedAt,
                    r.Societe,
                    r.Fonction,
                    r.Direction,
                    r.ModulesJson,
                    r.LockedByAdmin,
                    r.LockedAt
                })
                .ToListAsync();

            return Ok(requests);
        }
        [HttpGet("rejected")]
        public async Task<ActionResult<IEnumerable<UserAccessRequest>>> GetRejectedRequests()
        {
            var requests = await _context.UserAccessRequests
                .Where(r => r.State.Contains("Rejected"))
                .OrderByDescending(r => r.SubmittedAt)
                .Take(30)
                .Select(r => new {
                    r.Id,
                    r.UserName,
                    r.UserEmail,
                    r.ApplicationName,
                    r.State,
                    r.SubmittedAt,
                    r.Societe,
                    r.Fonction,
                    r.Direction,
                    r.ValidatorComment,
                    r.ModulesJson,
                    r.AdminComment
                })
                .ToListAsync();

            return Ok(requests);
        }
        [HttpPost("set-in-progress/{id}")]
        public async Task<IActionResult> SetRequestInProgress(int id)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.State != "Not viewed") return BadRequest("Already in progress or completed.");

            request.State = "In Progress";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserAccessRequest>>> SearchRequests([FromQuery] string query = "")
        {
            var lowerQuery = query.ToLower();

            var results = await _context.UserAccessRequests
                .Where(r =>
                    r.UserName.ToLower().Contains(lowerQuery) ||
                    r.ApplicationName.ToLower().Contains(lowerQuery)
                )
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();

            return Ok(results);
        }

        [HttpPost("validate/{id}")]
        public async Task<IActionResult> ValidateRequest(int id, [FromBody] ValidatorApprovalDto dto)
        {

            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound(new { message = "Request not found" });

            var now = DateTime.Now;
            string emailType = "";
            string emailSubject = "";
            string emailBody = "";
            var validatorName = await GetUserNameByEmail(dto.ValidatorEmail);
            if (request.Validateur1 == dto.ValidatorEmail && !request.ValidatedBy1)
            {
                request.ValidatedBy1 = true;
                request.State = string.IsNullOrWhiteSpace(request.Validateur2)
                    ? (string.IsNullOrWhiteSpace(request.Validateur3) ? "Pending Admin" : "Pending Validateur 3")
                    : "Pending Validateur 2";

                _context.RequestValidations.Add(new RequestValidation
                {
                    RequestId = id,
                    ValidatorEmail = dto.ValidatorEmail,
                    ValidatedAt = now,
                    Comment = dto.Comment 
                });
                emailType = "first_validation";
                emailSubject = "Request Validation Update";
                emailBody = _templateService.GetValidationEmail(
                    request.ApplicationName, 
                    request.State,
                    validatorName);
                await _emailService.SendValidationEmail(
                request.UserEmail,
                dto.ValidatorEmail,
                request.ApplicationName,
                request.State,
                validatorName,
                request.Id);
                await NotificationsController.CreateNotification(_context, 
                    request.UserEmail,
                    "Validation Update",
                    $"Your request for {request.ApplicationName} has been validated by the first validator",
                    "status",
                    id);
            }
            else if (request.Validateur3 == dto.ValidatorEmail && request.ValidatedBy1 && request.ValidatedBy2 && !request.ValidatedBy3)
            {
                request.ValidatedBy2 = true;
                request.State = string.IsNullOrWhiteSpace(request.Validateur3) ? "Pending Admin" : "Pending Validateur 3";

                _context.RequestValidations.Add(new RequestValidation
                {
                    RequestId = id,
                    ValidatorEmail = dto.ValidatorEmail,
                    ValidatedAt = now,
                    Comment = dto.Comment
                });
                emailType = "Second_validation";
                emailSubject = "Request Validation Update";
                emailBody = emailBody = _templateService.GetValidationEmail(
                                request.ApplicationName, 
                                request.State,
                                validatorName);
                await _emailService.SendValidationEmail(
                request.UserEmail,
                dto.ValidatorEmail,
                request.ApplicationName,
                request.State,
                validatorName,
                request.Id);
                await NotificationsController.CreateNotification(_context, 
                    request.UserEmail,
                    "Validation Update",
                    $"Your request for {request.ApplicationName} has been validated by the second validator",
                    "status",
                    id);
            }
            else if (request.Validateur3 == dto.ValidatorEmail && request.ValidatedBy1 && request.ValidatedBy2 && !request.ValidatedBy3)
            {
                request.ValidatedBy3 = true;
                request.State = "Pending Admin";

                _context.RequestValidations.Add(new RequestValidation
                {
                    RequestId = id,
                    ValidatorEmail = dto.ValidatorEmail,
                    ValidatedAt = now,
                    Comment = dto.Comment
                });
                emailType = "Validated";
                emailSubject = "Request Validation Update";
                emailBody = emailBody = _templateService.GetValidationEmail(
                                    request.ApplicationName, 
                                    request.State,
                                    validatorName);
                await _emailService.SendValidationEmail(
                request.UserEmail,
                dto.ValidatorEmail,
                request.ApplicationName,
                request.State,
                validatorName,
                request.Id);
                await NotificationsController.CreateNotification(_context, 
                    request.UserEmail,
                    "Validation Update",
                    $"Your request for {request.ApplicationName} has been validated by all validators",
                    "status",
                    id);
            }
            else
            {
                return BadRequest(new { message = "Invalid validator or already validated" });
            }
            await _emailService.SendAndRecordEmail(
            request.UserEmail,
            dto.ValidatorEmail,
            emailSubject,
            emailBody,
            request.Id,
            emailType);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Validation successful", nextState = request.State });
        }

        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptRequest(int id, [FromBody] string adminEmail)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();
            request.LockedByAdmin = adminEmail;
            request.LockedAt = DateTime.Now;
            request.State = "Accepted";
            string emailType = "Accepted";
            string emailSubject = "Request Accepted";
            string emailBody = $"Your request for {request.ApplicationName} has been Accepted by admin and is being processed";
            await NotificationsController.CreateNotification(_context, 
                request.UserEmail,
                "Request Accepted",
                $"Your request for {request.ApplicationName} has been accepted by admin and is being processed",
                "status",
                id);
            await _emailService.SendAndRecordEmail(
            request.UserEmail,
            adminEmail,
            emailSubject,
            emailBody,
            request.Id,
            emailType);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Request accepted and locked to admin." });
        }
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id, [FromBody] string adminEmail)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.LockedByAdmin != adminEmail)
            {
                return BadRequest(new { message = "Only the admin who accepted the request can approve it" });
            }
            string emailType = "Approved";
            string emailSubject = "Request Approved";
            string emailBody = $"Your request for {request.ApplicationName} has been approved by admin";
            request.State = "Finished";
            request.LockedByAdmin = null;
            request.LockedAt = null;
            await _emailService.SendAndRecordEmail(
            request.UserEmail,
            adminEmail,
            emailSubject,
            emailBody,
            request.Id,
            emailType);
            await NotificationsController.CreateNotification(_context, 
                request.UserEmail,
                "Request Approved",
                $"Your request for {request.ApplicationName} has been approved by admin",
                "status",
                id);
        
            await _context.SaveChangesAsync();
            return Ok(new { message = "Request approved and completed." });
        }
        [HttpPost("lock/{id}")]
        public async Task<IActionResult> LockRequest(int id, [FromBody] string adminEmail)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();

            // Check if already locked by another admin
            if (!string.IsNullOrEmpty(request.LockedByAdmin) && request.LockedByAdmin != adminEmail)
            {
                return BadRequest(new { message = $"Request is currently being processed by {request.LockedByAdmin}" });
            }

            request.LockedByAdmin = adminEmail;
            request.LockedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request locked successfully" });
            }

        [HttpPost("unlock/{id}")]
        public async Task<IActionResult> UnlockRequest(int id, [FromBody] string adminEmail)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();

            // Only the locking admin can unlock
            if (request.LockedByAdmin != adminEmail)
            {
                return BadRequest(new { message = "Only the admin who locked the request can unlock it" });
            }

            request.LockedByAdmin = null;
            request.LockedAt = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request unlocked successfully" });
        }

        [HttpGet("is-locked/{id}")]
        public async Task<IActionResult> IsRequestLocked(int id)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.LockedAt.HasValue && request.LockedAt.Value.AddMinutes(5) < DateTime.UtcNow)
            {
                request.LockedByAdmin = null;
                request.LockedAt = null;
                await _context.SaveChangesAsync();
            }

            return Ok(new { 
                isLocked = !string.IsNullOrEmpty(request.LockedByAdmin),
                lockedBy = request.LockedByAdmin,
                lockedAt = request.LockedAt
            });
        }

        [HttpPost("validator-deny/{id}")]
        public async Task<IActionResult> ValidatorDenyRequest(int id, [FromBody] ValidatorRejectionDto rejectionDto)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            var validatorEmail = rejectionDto.ValidatorEmail;
            var validatorName = await GetUserNameByEmail(validatorEmail);
            var now = DateTime.Now;
            bool isValidatorRejection = !string.IsNullOrEmpty(rejectionDto.ValidatorEmail);
                if (request.Validateur1 == rejectionDto.ValidatorEmail && !request.ValidatedBy1)
                {
                    request.State = "Rejected by Validator 1";
                    request.ValidatedBy1 = true; // Mark as validated (but rejected)
                }
                else if (request.Validateur2 == rejectionDto.ValidatorEmail && !request.ValidatedBy2)
                {
                    request.State = "Rejected by Validator 2";
                    request.ValidatedBy2 = true; // Mark as validated (but rejected)
                }
                else if (request.Validateur3 == rejectionDto.ValidatorEmail && !request.ValidatedBy3)
                {
                    request.State = "Rejected by Validator 3";
                    request.ValidatedBy3 = true; // Mark as validated (but rejected)
                }
                else
                {
                    return BadRequest("You are not authorized to reject this request");
                }
                request.ValidatorComment = rejectionDto.Comment;
                request.LockedByAdmin = null;
                request.LockedAt = null;
                // Add to validation history as a rejection
                _context.RequestValidations.Add(new RequestValidation
                {
                    RequestId = id,
                    ValidatorEmail = rejectionDto.ValidatorEmail,
                    ValidatedAt = now,
                    IsRejection = true,
                    Comment = rejectionDto.Comment
                });
            string emailType = "Reject";
            string emailSubject = "Request Rejected";
            string emailBody = $"Your request for {request.ApplicationName} has been rejected by {(isValidatorRejection ? "validator" : "admin")}";
            await _emailService.SendAndRecordEmail(
            request.UserEmail,
            rejectionDto.ValidatorEmail,
            emailSubject,
            emailBody,
            request.Id,
            emailType);
            await NotificationsController.CreateNotification(_context, 
                request.UserEmail,
                "Request Rejected",
                $"Your request for {request.ApplicationName} has been rejected by {(isValidatorRejection ? "validator" : "admin")}",
                "status",
                id);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Request rejected and archived."});
        }
        [HttpPost("admin-deny/{id}")]
        public async Task<IActionResult> AdminDenyRequest(int id, [FromBody] AdminRejectionDto rejectionDto)
        {
            var request = await _context.UserAccessRequests.FindAsync(id);
            var adminName = await GetUserNameByEmail(rejectionDto.AdminEmail);
            if (request == null) return NotFound();
    
            if (!string.IsNullOrEmpty(request.LockedByAdmin))
            {
                if (request.LockedByAdmin != rejectionDto.AdminEmail)
                {
                    return BadRequest(new { message = "Only the admin who accepted the request can reject it" });
                }
            }
    
            request.State = "Rejected";
            request.AdminComment = rejectionDto.Comment;
            request.LockedByAdmin = null;
            request.LockedAt = null;
            await _emailService.SendRejectionEmail(
            request.UserEmail,
            rejectionDto.AdminEmail,
            request.ApplicationName,
            rejectionDto.Comment,
            adminName,
            request.Id);
            await NotificationsController.CreateNotification(_context, 
                request.UserEmail,
                "Request Rejected",
                $"Your request for {request.ApplicationName} has been rejected by admin",
                "status",
                id);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Request rejected and unlocked." });
        }
        private async Task<string> GetUserNameByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user?.Name ?? email;
    }
        
    }
    public class ValidatorApprovalDto
{
    public string ValidatorEmail { get; set; }
    public string? Comment { get; set; }
}

    public class ValidatorRejectionDto
    {
    public string ValidatorEmail { get; set; }
    public string Comment { get; set; }
    }
    public class AdminRejectionDto
{
    public string AdminEmail { get; set; }
    public string Comment { get; set; }
}
}
