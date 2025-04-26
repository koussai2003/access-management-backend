using System.ComponentModel.DataAnnotations;

namespace AccessManagementAPI.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string LogoUrl { get; set; }
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
