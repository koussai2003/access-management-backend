using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccessManagementAPI.Models
{
    public class UserColumnInputModel
    {
        [Required]
        [MaxLength(50)]
        public string ColumnType { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Value { get; set; }
    }
}