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

    public Address? ContactAddress { get; set; }

    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }

    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
