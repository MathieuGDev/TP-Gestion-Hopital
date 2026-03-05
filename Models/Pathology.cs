using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Pathology
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation vers Patient
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
