using tp_hospital.Data;

namespace tp_hospital.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HospitalDbContext _context;

    public IPatientRepository      Patients      { get; }
    public IConsultationRepository Consultations { get; }

    public UnitOfWork(HospitalDbContext context)
    {
        _context      = context;
        Patients      = new PatientRepository(context);
        Consultations = new ConsultationRepository(context);
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
