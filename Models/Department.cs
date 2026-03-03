using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int? HeadDoctorId { get; set; }
    public Doctor? HeadDoctor { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
