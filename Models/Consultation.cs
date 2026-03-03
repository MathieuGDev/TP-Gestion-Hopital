using System;
using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

public class Consultation
{
    [Key]
    public int Id { get; set; }

    // Relation vers Patient
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    // Relation vers Doctor
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    [Required]
    public DateTime AppointmentDate { get; set; }

    public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;

    public string Notes { get; set; } = string.Empty;
}

public enum ConsultationStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}
