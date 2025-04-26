using System.ComponentModel.DataAnnotations;

namespace AccessManagementAPI.Models
{
public class ModuleFunction
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }

    public bool Consultation { get; set; }
    public bool Creation { get; set; }
    public bool Modification { get; set; }
    public bool Suppression { get; set; }
    public bool Validation { get; set; }

    public int ModuleId { get; set; }
    public Module Module { get; set; }
}
}