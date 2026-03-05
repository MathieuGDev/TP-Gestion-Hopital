using tp_hospital.Models;

namespace tp_hospital.Repositories;

public interface IConsultationRepository
{
    Task<Consultation?> GetByIdAsync(int id);
    Task<bool> SlotExistsAsync(int patientId, int doctorId, DateTime appointmentDate, int? excludeId = null);
    Task<bool> HasDoctorOverlapAsync(int doctorId, DateTime appointmentDate, int? excludeId = null);
    Task<IEnumerable<Consultation>> GetUpcomingByPatientAsync(int patientId);
    Task<IEnumerable<Consultation>> GetTodayByDoctorAsync(int doctorId);
    Task<Consultation> CreateAsync(Consultation consultation);
    Task<Consultation> UpdateAsync(Consultation consultation);
    Task DeleteAsync(Consultation consultation);
}
