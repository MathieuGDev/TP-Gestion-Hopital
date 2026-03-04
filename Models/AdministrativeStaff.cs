using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class AdministrativeStaff : MedicalStaff
{
    [Required]
    [MaxLength(100)]
    public string Function { get; set; } = string.Empty;
}
