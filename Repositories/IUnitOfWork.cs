namespace tp_hospital.Repositories;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IConsultationRepository Consultations { get; }
    Task<int> SaveChangesAsync();
}
