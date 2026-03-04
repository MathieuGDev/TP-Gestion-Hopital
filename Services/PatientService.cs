using Microsoft.EntityFrameworkCore;
using tp_hospital.Models;
using tp_hospital.Repositories;

namespace tp_hospital.Services;

public class PatientService
{
    private readonly IUnitOfWork _uow;

    public PatientService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ServiceResult<Patient>> CreatePatientAsync(Patient patient)
    {
        if (await _uow.Patients.EmailExistsAsync(patient.Email))
            return ServiceResult<Patient>.Conflict(
                $"L'email '{patient.Email}' est deja utilise.");

        if (await _uow.Patients.FolderNumberExistsAsync(patient.FolderNumber))
            return ServiceResult<Patient>.Conflict(
                $"Le numero de dossier '{patient.FolderNumber}' est deja utilise.");

        await _uow.Patients.CreateAsync(patient);
        await _uow.SaveChangesAsync();

        return ServiceResult<Patient>.Created(patient);
    }

    // Met a jour un patient existant.
    public async Task<ServiceResult<Patient>> UpdatePatientAsync(int id, Patient updatedPatient)
    {
        var existing = await _uow.Patients.GetByIdAsync(id);
        if (existing == null)
            return ServiceResult<Patient>.NotFound(
                $"Aucun patient trouve avec l'ID {id}.");

        if (await _uow.Patients.EmailExistsAsync(updatedPatient.Email, excludeId: id))
            return ServiceResult<Patient>.Conflict(
                $"L'email '{updatedPatient.Email}' est deja utilise par un autre patient.");

        if (await _uow.Patients.FolderNumberExistsAsync(updatedPatient.FolderNumber, excludeId: id))
            return ServiceResult<Patient>.Conflict(
                $"Le numero de dossier '{updatedPatient.FolderNumber}' est deja utilise par un autre patient.");

        existing.FirstName    = updatedPatient.FirstName;
        existing.LastName     = updatedPatient.LastName;
        existing.DateOfBirth  = updatedPatient.DateOfBirth;
        existing.Address      = updatedPatient.Address;
        existing.Email        = updatedPatient.Email;
        existing.FolderNumber = updatedPatient.FolderNumber;
        existing.RowVersion   = updatedPatient.RowVersion + 1;

        try
        {
            await _uow.Patients.UpdateAsync(existing);
            await _uow.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<Patient>.ConcurrencyConflict(
                "Conflit de concurrence : le patient a ete modifie par un autre utilisateur. Rechargez et reessayez.");
        }

        return ServiceResult<Patient>.Success(existing);
    }

    // Supprime un patient si et seulement si il n'a pas de consultations.
    public async Task<ServiceResult<bool>> DeletePatientAsync(int id)
    {
        var patient = await _uow.Patients.GetByIdAsync(id);
        if (patient == null)
            return ServiceResult<bool>.NotFound($"Aucun patient trouve avec l'ID {id}.");

        if (patient.Consultations.Any())
            return ServiceResult<bool>.Conflict(
                "Impossible de supprimer ce patient : il possede des consultations.");

        await _uow.Patients.DeleteAsync(patient);
        await _uow.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }
}
