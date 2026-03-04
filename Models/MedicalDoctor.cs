using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class MedicalDoctor : MedicalStaff // Héritage de MedicalStaff
{
    [Required]
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;
}
