using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessManagementAPI.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserEmail { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public string NotificationType { get; set; }
        
        public int? RelatedRequestId { get; set; }
        
        [ForeignKey("RelatedRequestId")]
        public UserAccessRequest RelatedRequest { get; set; }
    }
}