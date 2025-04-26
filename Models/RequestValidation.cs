using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessManagementAPI.Models
{
    public class RequestValidation
    {
    [Key]
    public int Id { get; set; }

    public int RequestId { get; set; } // FK to UserAccessRequest
    public string ValidatorEmail { get; set; }
    public DateTime ValidatedAt { get; set; }
    public bool IsRejection { get; set; }
    public string? Comment { get; set; }

    [ForeignKey("RequestId")]
    public UserAccessRequest Request { get; set; }
}


}