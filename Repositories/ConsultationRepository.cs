using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;

namespace tp_hospital.Repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly HospitalDbContext _context;

    public ConsultationRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> GetByIdAsync(int id)
    {
        return await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Patient)
            .Include(c => c.Doctor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Verifie si la combinaison (patient, medecin, date) existe deja.
    public async Task<bool> SlotExistsAsync(
        int patientId, int doctorId, DateTime appointmentDate, int? excludeId = null)
    {
        return await _context.Consultations
            .AsNoTracking()
            .AnyAsync(c => c.PatientId      == patientId
                        && c.DoctorId       == doctorId
                        && c.AppointmentDate == appointmentDate
                        && (excludeId == null || c.Id != excludeId));
    }

    // Verifie si le medecin a deja une consultation a cette date/heure.
    public async Task<bool> HasDoctorOverlapAsync(
        int doctorId, DateTime appointmentDate, int? excludeId = null)
    {
        return await _context.Consultations
            .AsNoTracking()
            .AnyAsync(c => c.DoctorId        == doctorId
                        && c.AppointmentDate == appointmentDate
                        && c.Status          != ConsultationStatus.Cancelled
                        && (excludeId == null || c.Id != excludeId));
    }

    public async Task<IEnumerable<Consultation>> GetUpcomingByPatientAsync(int patientId)
    {
        return await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Doctor)
            .Where(c => c.PatientId == patientId
                     && c.AppointmentDate >= DateTime.Today
                     && c.Status == ConsultationStatus.Scheduled)
            .OrderBy(c => c.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultation>> GetTodayByDoctorAsync(int doctorId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        return await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId
                     && c.AppointmentDate >= today
                     && c.AppointmentDate <  tomorrow)
            .OrderBy(c => c.AppointmentDate)
            .ToListAsync();
    }

    public Task<Consultation> CreateAsync(Consultation consultation)
    {
        _context.Consultations.Add(consultation);
        return Task.FromResult(consultation);
    }

    public Task<Consultation> UpdateAsync(Consultation consultation)
    {
        _context.Consultations.Update(consultation);
        return Task.FromResult(consultation);
    }

    public Task DeleteAsync(Consultation consultation)
    {
        _context.Consultations.Remove(consultation);
        return Task.CompletedTask;
    }
}
