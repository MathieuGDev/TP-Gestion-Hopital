using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;

namespace tp_hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PatientController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET api/patient?page=1&pageSize=10
        // Liste tous les patients par ordre alphabetique, avec pagination
        [HttpGet]
        public async Task<ActionResult> GetPatients(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Les parametres page et pageSize doivent etre superieurs a 0." });

            var query = _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName);

            int total = await query.CountAsync();

            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = patients });
        }

        // GET api/patient/{id}
        // Récupère un patient par son ID avec toutes ses consultations
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPatient(int id)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .Include(p => p.Consultations.OrderByDescending(c => c.AppointmentDate))
                    .ThenInclude(c => c.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound(new { message = $"Aucun patient trouve avec l'ID {id}." });

            return Ok(new
            {
                patient.Id,
                patient.FolderNumber,
                patient.FirstName,
                patient.LastName,
                patient.DateOfBirth,
                patient.Email,
                patient.PhoneNumber,
                Address = patient.Address == null ? null : new
                {
                    patient.Address.Street,
                    patient.Address.City,
                    patient.Address.PostalCode,
                    patient.Address.Country
                },
                Consultations = patient.Consultations.Select(c => new
                {
                    c.Id,
                    c.AppointmentDate,
                    c.Status,
                    c.Notes,
                    Doctor = c.Doctor == null ? null : new { c.Doctor.Id, c.Doctor.FirstName, c.Doctor.LastName, c.Doctor.Specialty }
                })
            });
        }

        // GET api/patient/search?name=dupont&page=1&pageSize=10
        // Recherche par nom ou prenom
        [HttpGet("search")]
        public async Task<ActionResult> SearchPatients(string name, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Le parametre 'name' est requis." });

            var pattern = $"%{name}%";

            var query = _context.Patients
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.LastName, pattern)
                         || EF.Functions.Like(p.FirstName, pattern))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName);

            int total = await query.CountAsync();

            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = patients });
        }

        // POST api/patient
        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool emailExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(p => p.Email == patient.Email);
            if (emailExists)
                return Conflict(new { message = $"L'email '{patient.Email}' est deja utilise." });

            bool folderExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(p => p.FolderNumber == patient.FolderNumber);
            if (folderExists)
                return Conflict(new { message = $"Le numero de dossier '{patient.FolderNumber}' est deja utilise." });

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT api/patient/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            if (id != updatedPatient.Id)
                return BadRequest(new { message = "L'ID dans l'URL doit correspondre a l'ID du patient." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPatient = await _context.Patients.FindAsync(id);
            if (existingPatient == null)
                return NotFound(new { message = $"Aucun patient trouve avec l'ID {id}." });

            bool emailExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(p => p.Email == updatedPatient.Email && p.Id != id);
            if (emailExists)
                return Conflict(new { message = $"L'email '{updatedPatient.Email}' est deja utilise par un autre patient." });

            bool folderExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(p => p.FolderNumber == updatedPatient.FolderNumber && p.Id != id);
            if (folderExists)
                return Conflict(new { message = $"Le numero de dossier '{updatedPatient.FolderNumber}' est deja utilise par un autre patient." });

            existingPatient.FirstName    = updatedPatient.FirstName;
            existingPatient.LastName     = updatedPatient.LastName;
            existingPatient.DateOfBirth  = updatedPatient.DateOfBirth;
            existingPatient.Address      = updatedPatient.Address;
            existingPatient.Email        = updatedPatient.Email;
            existingPatient.FolderNumber = updatedPatient.FolderNumber;
            existingPatient.PhoneNumber  = updatedPatient.PhoneNumber;

            // Détection des conflits :
            // Si la ligne a ete modifiee entre-temps, aucune ligne n'est affectee
            _context.Entry(existingPatient)
                .Property(p => p.RowVersion)
                .OriginalValue = updatedPatient.RowVersion;
            existingPatient.RowVersion = updatedPatient.RowVersion + 1;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var dbEntry = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                return Conflict(new
                {
                    message = "Conflit de concurrence : le patient a ete modifie par un autre utilisateur depuis votre derniere lecture. Rechargez les donnees et reessayez.",
                    currentRowVersion = dbEntry?.RowVersion
                });
            }

            return Ok(existingPatient);
        }

        // DELETE api/patient/{id}
        // Refuse la suppression si le patient a des consultations
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = $"Aucun patient trouve avec l'ID {id}." });

            bool hasConsultations = await _context.Consultations
                .AsNoTracking()
                .AnyAsync(c => c.PatientId == id);

            if (hasConsultations)
                return Conflict(new { message = "Impossible de supprimer ce patient : il possede des consultations. Supprimez-les d'abord." });

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
