using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;

namespace tp_hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public ConsultationController(HospitalDbContext context)
        {
            _context = context;
        }

        // POST api/consultation
        // Planifier une nouvelle consultation
        [HttpPost]
        public async Task<ActionResult<Consultation>> CreateConsultation([FromBody] Consultation consultation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool patientExists = await _context.Patients.AsNoTracking().AnyAsync(p => p.Id == consultation.PatientId);
            if (!patientExists)
                return NotFound(new { message = $"Aucun patient trouve avec l'ID {consultation.PatientId}." });

            bool doctorExists = await _context.Doctors.AsNoTracking().AnyAsync(d => d.Id == consultation.DoctorId);
            if (!doctorExists)
                return NotFound(new { message = $"Aucun medecin trouve avec l'ID {consultation.DoctorId}." });

            // Contrainte : pas deux consultations au meme moment pour le meme patient avec le meme medecin
            bool conflict = await _context.Consultations
                .AsNoTracking()
                .AnyAsync(c => c.PatientId == consultation.PatientId
                            && c.DoctorId  == consultation.DoctorId
                            && c.AppointmentDate == consultation.AppointmentDate);
            if (conflict)
                return Conflict(new { message = "Ce patient a deja une consultation avec ce medecin a cette date et heure." });

            consultation.Status = ConsultationStatus.Scheduled;

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();

            // Recharge la consultation avec les données liées pour la réponse
            var result = await _context.Consultations
                .Include(c => c.Patient)
                .Include(c => c.Doctor)
                .FirstAsync(c => c.Id == consultation.Id);

            return CreatedAtAction(nameof(CreateConsultation), new { id = result.Id }, result);
        }

        // PUT api/consultation/{id}/status
        // Modifier le statut d'une consultation
        [HttpPut("{id}/status")]
        public async Task<ActionResult<Consultation>> UpdateStatus(int id, [FromBody] ConsultationStatus status)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
                return NotFound(new { message = $"Aucune consultation trouvee avec l'ID {id}." });

            if (consultation.Status == ConsultationStatus.Cancelled)
                return BadRequest(new { message = "Impossible de modifier le statut d'une consultation annulee." });

            consultation.Status = status;
            await _context.SaveChangesAsync();

            return Ok(consultation);
        }

        // DELETE api/consultation/{id}
        // Annuler une consultation en passant le statut a "Cancelled", sans supprimer l'enregistrement de la consultation
        [HttpDelete("{id}")]
        public async Task<ActionResult<Consultation>> CancelConsultation(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
                return NotFound(new { message = $"Aucune consultation trouvee avec l'ID {id}." });

            if (consultation.Status == ConsultationStatus.Cancelled)
                return BadRequest(new { message = "Cette consultation est deja annulee." });

            if (consultation.Status == ConsultationStatus.Completed)
                return BadRequest(new { message = "Impossible d'annuler une consultation deja terminee." });

            consultation.Status = ConsultationStatus.Cancelled;
            await _context.SaveChangesAsync();

            return Ok(consultation);
        }

        // GET api/consultation/patient/{patientId}/upcoming?page=1&pageSize=10
        // Lister les consultations a venir pour un patient
        [HttpGet("patient/{patientId}/upcoming")]
        public async Task<ActionResult> GetUpcomingForPatient(int patientId, int page = 1, int pageSize = 10)
        {
            bool patientExists = await _context.Patients.AsNoTracking().AnyAsync(p => p.Id == patientId);
            if (!patientExists)
                return NotFound(new { message = $"Aucun patient trouve avec l'ID {patientId}." });

            var now = DateTime.Now;

            var query = _context.Consultations
                .AsNoTracking()
                .Where(c => c.PatientId == patientId
                         && c.AppointmentDate > now
                         && c.Status != ConsultationStatus.Cancelled)
                .Include(c => c.Doctor)
                .OrderBy(c => c.AppointmentDate);

            int total = await query.CountAsync();

            var consultations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.AppointmentDate,
                    c.Status,
                    c.Notes,
                    Doctor = new { c.Doctor!.Id, c.Doctor.FirstName, c.Doctor.LastName, c.Doctor.Specialty }
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = consultations });
        }

        // GET api/consultation/doctor/{doctorId}/today?page=1&pageSize=10
        // Lister les consultations du jour pour un medecin
        [HttpGet("doctor/{doctorId}/today")]
        public async Task<ActionResult> GetTodayForDoctor(int doctorId, int page = 1, int pageSize = 10)
        {
            bool doctorExists = await _context.Doctors.AsNoTracking().AnyAsync(d => d.Id == doctorId);
            if (!doctorExists)
                return NotFound(new { message = $"Aucun medecin trouve avec l'ID {doctorId}." });

            var today    = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = _context.Consultations
                .AsNoTracking()
                .Where(c => c.DoctorId == doctorId
                         && c.AppointmentDate >= today
                         && c.AppointmentDate <  tomorrow
                         && c.Status != ConsultationStatus.Cancelled)
                .Include(c => c.Patient)
                .OrderBy(c => c.AppointmentDate);

            int total = await query.CountAsync();

            var consultations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.AppointmentDate,
                    c.Status,
                    c.Notes,
                    Patient = new { c.Patient!.Id, c.Patient.FirstName, c.Patient.LastName, c.Patient.FolderNumber }
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = consultations });
        }
    }
}

