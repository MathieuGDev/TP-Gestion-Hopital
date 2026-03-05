using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;

namespace tp_hospital.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly HospitalDbContext _context;

    public PatientRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Patient> Data, int Total)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        int total = await query.CountAsync();
        var data  = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, total);
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients
            .AsNoTracking()
            .Include(p => p.Consultations.OrderByDescending(c => c.AppointmentDate))
                .ThenInclude(c => c.Doctor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Patient> Data, int Total)> SearchByNameAsync(
        string name, int page, int pageSize)
    {
        var lName = name.ToLower();
        var query = _context.Patients
            .AsNoTracking()
            .Where(p => p.LastName.ToLower().Contains(lName)
                     || p.FirstName.ToLower().Contains(lName))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        int total = await query.CountAsync();
        var data  = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, total);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Email == email && (excludeId == null || p.Id != excludeId));
    }

    public async Task<bool> FolderNumberExistsAsync(int folderNumber, int? excludeId = null)
    {
        return await _context.Patients
            .AsNoTracking()
            .AnyAsync(p => p.FolderNumber == folderNumber && (excludeId == null || p.Id != excludeId));
    }

    public Task<Patient> CreateAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        return Task.FromResult(patient);
    }

    public Task<Patient> UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        return Task.FromResult(patient);
    }

    public Task DeleteAsync(Patient patient)
    {
        _context.Patients.Remove(patient);
        return Task.CompletedTask;
    }
}
