using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Fonction { get; set; }

        [MaxLength(100)]
        public string Societe { get; set; }

        [MaxLength(100)]
        public string Direction { get; set; }

        [MaxLength(100)]
        public string Statut { get; set; } = "actif"; // 'actif' or 'inactif'

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public bool MustChangePassword { get; set; } = true;

        [Required]
        public string Role { get; set; } = "user";

        [Required]
        public string Validateur1 { get; set; }

        public string? Validateur2 { get; set; }

        public string? Validateur3 { get; set; }
        [NotMapped]
        public string? Password { get; set; }
    }
}
