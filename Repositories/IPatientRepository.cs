using tp_hospital.Models;

namespace tp_hospital.Repositories;

public interface IPatientRepository
{
    Task<(IEnumerable<Patient> Data, int Total)> GetAllAsync(int page, int pageSize);
    Task<Patient?> GetByIdAsync(int id);
    Task<(IEnumerable<Patient> Data, int Total)> SearchByNameAsync(string name, int page, int pageSize);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> FolderNumberExistsAsync(int folderNumber, int? excludeId = null);
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task DeleteAsync(Patient patient);
}
