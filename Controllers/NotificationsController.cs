using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Data;
using AccessManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications/{userEmail}
        [HttpGet("{userEmail}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(string userEmail)
        {
            return await _context.Notifications
                .Where(n => n.UserEmail == userEmail)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // GET: api/notifications/unread-count/{userEmail}
        [HttpGet("unread-count/{userEmail}")]
        public async Task<ActionResult<int>> GetUnreadCount(string userEmail)
        {
            return await _context.Notifications
                .Where(n => n.UserEmail == userEmail && !n.IsRead)
                .CountAsync();
        }

        // PUT: api/notifications/mark-as-read/{id}
        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to create notifications (called from other controllers)
        public static async Task CreateNotification(ApplicationDbContext context, 
            string userEmail, string title, string message, string type, int? requestId = null)
        {
            var notification = new Notification
            {
                UserEmail = userEmail,
                Title = title,
                Message = message,
                NotificationType = type,
                RelatedRequestId = requestId
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
        }
    }
}