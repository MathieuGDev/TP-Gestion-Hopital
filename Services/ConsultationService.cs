using tp_hospital.Models;
using tp_hospital.Repositories;

namespace tp_hospital.Services;

public class ConsultationService
{
    private readonly IUnitOfWork _uow;

    public ConsultationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ServiceResult<Consultation>> ScheduleAsync(Consultation consultation)
    {
        if (consultation.AppointmentDate < DateTime.Now)
            return ServiceResult<Consultation>.Conflict(
                "La date du rendez-vous doit etre dans le futur.");

        if (await _uow.Consultations.SlotExistsAsync(
                consultation.PatientId,
                consultation.DoctorId,
                consultation.AppointmentDate))
            return ServiceResult<Consultation>.Conflict(
                "Ce patient a deja une consultation avec ce medecin a cette date.");

        if (await _uow.Consultations.HasDoctorOverlapAsync(
                consultation.DoctorId,
                consultation.AppointmentDate))
            return ServiceResult<Consultation>.Conflict(
                "Le medecin a deja une consultation planifiee a cette date et heure.");

        consultation.Status = ConsultationStatus.Scheduled;

        await _uow.Consultations.CreateAsync(consultation);
        await _uow.SaveChangesAsync();

        return ServiceResult<Consultation>.Created(consultation);
    }

    /// <summary>
    /// Change le statut d'une consultation.
    /// Les transitions invalides sont refusees.
    /// </summary>
    public async Task<ServiceResult<Consultation>> UpdateStatusAsync(int id, ConsultationStatus newStatus)
    {
        var consultation = await _uow.Consultations.GetByIdAsync(id);
        if (consultation == null)
            return ServiceResult<Consultation>.NotFound(
                $"Aucune consultation trouvee avec l'ID {id}.");

        if (consultation.Status == ConsultationStatus.Cancelled)
            return ServiceResult<Consultation>.Conflict(
                "Impossible de modifier une consultation annulee.");

        if (consultation.Status == ConsultationStatus.Completed)
            return ServiceResult<Consultation>.Conflict(
                "Impossible de modifier une consultation terminee.");

        consultation.Status = newStatus;
        await _uow.Consultations.UpdateAsync(consultation);
        await _uow.SaveChangesAsync();

        return ServiceResult<Consultation>.Success(consultation);
    }
}
