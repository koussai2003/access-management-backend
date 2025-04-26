using System.ComponentModel.DataAnnotations;

namespace AccessManagementAPI.Models
{
public class Module
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int ApplicationId { get; set; }
    public Application Application { get; set; }

    public ICollection<ModuleFunction> Functions { get; set; } = new List<ModuleFunction>();
}
}
