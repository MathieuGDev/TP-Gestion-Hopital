using System;
using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Patient
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FolderNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [DateInPast]
    public DateTime DateOfBirth { get; set; }

    public Address? Address { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // detecte si un autre utilisateur a sauvegarde entre-temps
    [ConcurrencyCheck]
    public uint RowVersion { get; set; } = 0;

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}