using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI6.Models
{
    public class Employee
    {
        [Key]       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Address { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? CompanyName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? Designation { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
    }
}
