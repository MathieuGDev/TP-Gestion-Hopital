using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Nurse : MedicalStaff // Héritage de MedicalStaff
{
    [Required]
    [MaxLength(100)]
    public string Service { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Grade { get; set; } = string.Empty;
}
