using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessManagementAPI.Models
{
    public class EmailNotification
    {
        public int Id { get; set; }
        public string RecipientEmail { get; set; }
        public string SenderEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public int RelatedRequestId { get; set; }
        public string EmailType { get; set; }
        
        public UserAccessRequest RelatedRequest { get; set; }
    }
}