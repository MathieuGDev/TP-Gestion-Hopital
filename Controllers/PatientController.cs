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

        // GET api/patient/search?name=dupont&page=1&pageSize=10
        // Recherche par nom ou prenom
        [HttpGet("search")]
        public async Task<ActionResult> SearchPatients(string name, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Le parametre 'name' est requis." });

            var query = _context.Patients
                .AsNoTracking()
                .Where(p => p.LastName.ToLower().Contains(name.ToLower())
                         || p.FirstName.ToLower().Contains(name.ToLower()))
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

            return CreatedAtAction(nameof(CreatePatient), new { id = patient.Id }, patient);
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

            await _context.SaveChangesAsync();
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
