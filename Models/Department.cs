using System;
using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Department
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
}
