using System.ComponentModel.DataAnnotations;
namespace AccessManagementAPI.Models
{
    public class UserInputModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Fonction { get; set; }
        public string Societe { get; set; }
        public string Direction { get; set; }
        public string Statut { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Validateur1 { get; set; }
        public string? Validateur2 { get; set; }
        public string? Validateur3 { get; set; }
    }
}
