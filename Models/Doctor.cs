using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Doctor
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
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    // clé étrangère vers le model Department
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}
