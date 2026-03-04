using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;

namespace tp_hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DoctorController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET api/doctor?page=1&pageSize=10
        // Liste tous les médecins par ordre alphabétique avec leur département
        [HttpGet]
        public async Task<ActionResult> GetDoctors(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Les parametres page et pageSize doivent etre superieurs a 0." });

            var query = _context.Doctors
                .AsNoTracking()
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName);

            int total = await query.CountAsync();

            var doctors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.Id,
                    d.FirstName,
                    d.LastName,
                    d.Specialty,
                    d.LicenseNumber,
                    Department = d.Department == null ? null : new { d.Department.Id, d.Department.Name }
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = doctors });
        }

        // GET api/doctor/{id}
        // Récupère un médecin par son ID avec son département et ses consultations
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Include(d => d.Consultations.OrderByDescending(c => c.AppointmentDate))
                    .ThenInclude(c => c.Patient)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound(new { message = $"Aucun medecin trouve avec l'ID {id}." });

            return Ok(new
            {
                doctor.Id,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialty,
                doctor.LicenseNumber,
                Department = doctor.Department == null ? null : new { doctor.Department.Id, doctor.Department.Name },
                Consultations = doctor.Consultations.Select(c => new
                {
                    c.Id,
                    c.AppointmentDate,
                    c.Status,
                    c.Notes,
                    Patient = c.Patient == null ? null : new { c.Patient.Id, c.Patient.FirstName, c.Patient.LastName, c.Patient.FolderNumber }
                })
            });
        }

        // GET api/doctor/search?name=martin&page=1&pageSize=10
        // Recherche par nom, prénom ou spécialité
        [HttpGet("search")]
        public async Task<ActionResult> SearchDoctors(string name, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Le parametre 'name' est requis." });

            var query = _context.Doctors
                .AsNoTracking()
                .Where(d => d.LastName.ToLower().Contains(name.ToLower())
                         || d.FirstName.ToLower().Contains(name.ToLower())
                         || d.Specialty.ToLower().Contains(name.ToLower()))
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName);

            int total = await query.CountAsync();

            var doctors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.Id,
                    d.FirstName,
                    d.LastName,
                    d.Specialty,
                    d.LicenseNumber,
                    Department = d.Department == null ? null : new { d.Department.Id, d.Department.Name }
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = doctors });
        }

        // POST api/doctor
        // Créer un nouveau médecin
        [HttpPost]
        public async Task<ActionResult<Doctor>> CreateDoctor([FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool licenseExists = await _context.Doctors
                .AsNoTracking()
                .AnyAsync(d => d.LicenseNumber == doctor.LicenseNumber);
            if (licenseExists)
                return Conflict(new { message = $"Le numero de licence '{doctor.LicenseNumber}' est deja utilise." });

            bool departmentExists = await _context.Departments
                .AsNoTracking()
                .AnyAsync(dep => dep.Id == doctor.DepartmentId);
            if (!departmentExists)
                return NotFound(new { message = $"Aucun departement trouve avec l'ID {doctor.DepartmentId}." });

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var result = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .FirstAsync(d => d.Id == doctor.Id);

            return CreatedAtAction(nameof(GetDoctor), new { id = result.Id }, result);
        }

        // PUT api/doctor/{id}
        // Modifier un médecin existant
        [HttpPut("{id}")]
        public async Task<ActionResult<Doctor>> UpdateDoctor(int id, [FromBody] Doctor updatedDoctor)
        {
            if (id != updatedDoctor.Id)
                return BadRequest(new { message = "L'ID dans l'URL doit correspondre a l'ID du medecin." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Doctors.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Aucun medecin trouve avec l'ID {id}." });

            bool licenseExists = await _context.Doctors
                .AsNoTracking()
                .AnyAsync(d => d.LicenseNumber == updatedDoctor.LicenseNumber && d.Id != id);
            if (licenseExists)
                return Conflict(new { message = $"Le numero de licence '{updatedDoctor.LicenseNumber}' est deja utilise par un autre medecin." });

            bool departmentExists = await _context.Departments
                .AsNoTracking()
                .AnyAsync(dep => dep.Id == updatedDoctor.DepartmentId);
            if (!departmentExists)
                return NotFound(new { message = $"Aucun departement trouve avec l'ID {updatedDoctor.DepartmentId}." });

            existing.FirstName     = updatedDoctor.FirstName;
            existing.LastName      = updatedDoctor.LastName;
            existing.Specialty     = updatedDoctor.Specialty;
            existing.LicenseNumber = updatedDoctor.LicenseNumber;
            existing.DepartmentId  = updatedDoctor.DepartmentId;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE api/doctor/{id}
        // Refuse la suppression si le médecin a des consultations
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound(new { message = $"Aucun medecin trouve avec l'ID {id}." });

            bool hasConsultations = await _context.Consultations
                .AsNoTracking()
                .AnyAsync(c => c.DoctorId == id);
            if (hasConsultations)
                return Conflict(new { message = "Impossible de supprimer ce medecin : il possede des consultations. Supprimez-les d'abord." });

            bool isHeadDoctor = await _context.Departments
                .AsNoTracking()
                .AnyAsync(dep => dep.HeadDoctorId == id);
            if (isHeadDoctor)
                return Conflict(new { message = "Impossible de supprimer ce medecin : il est chef d'un departement. Reassignez le chef de service d'abord." });

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
