using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_hospital.Models;
public abstract class MedicalStaff
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime HireDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Salary { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
