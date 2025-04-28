using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccessManagementAPI.Models
{
    public class UserColumn
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ColumnType { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Value { get; set; }
    }
}