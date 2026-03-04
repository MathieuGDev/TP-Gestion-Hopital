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

    /// <summary>Adresse de contact du département (type complexe / Owned Entity).</summary>
    public Address? ContactAddress { get; set; }

    // ── Hiérarchie de départements (auto-référence) ──────────────────────────
    /// <summary>Département parent (null si département racine).</summary>
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }

    /// <summary>Sous-départements rattachés à ce département.</summary>
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
