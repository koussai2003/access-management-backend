using System.ComponentModel.DataAnnotations;
namespace AccessManagementAPI.Models
{
public class CreateTemplateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ApplicationName { get; set; }
    public string ModulesJson { get; set; }
    
}
}