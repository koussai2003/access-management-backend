using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccessManagementAPI.Models 
{
public class UserAccessRequest
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }

    public string Societe { get; set; }
    public string Fonction { get; set; }
    public string Direction { get; set; }
    
    public string ApplicationName { get; set; }
    public string State { get; set; } = "Pending Validateur 1";
    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    public string ModulesJson { get; set; }

    public string? AdminComment { get; set; }
    public string? LockedByAdmin { get; set; }
    public DateTime? LockedAt { get; set; }
    public string Validateur1 { get; set; }
    public string? Validateur2 { get; set; }
    public string? Validateur3 { get; set; }
    public string? ValidatorComment { get; set; }

    public bool ValidatedBy1 { get; set; } = false;
    public bool ValidatedBy2 { get; set; } = false;
    public bool ValidatedBy3 { get; set; } = false;
}

}