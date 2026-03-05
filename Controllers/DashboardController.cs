using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp_hospital.Data;
using tp_hospital.Models;
using DepartmentStatViewModel = tp_hospital.Models.DepartmentStatViewModel;

namespace tp_hospital.Controllers;

public class DashboardController : Controller
{
    private readonly HospitalDbContext _context;

    public DashboardController(HospitalDbContext context)
    {
        _context = context;
    }

    // GET /Dashboard  — tableau de bord global
    public async Task<IActionResult> Index()
    {
        ViewBag.PatientCount      = await _context.Patients.CountAsync();
        ViewBag.DoctorCount       = await _context.Doctors.CountAsync();
        ViewBag.ConsultationCount = await _context.Consultations.CountAsync();
        ViewBag.UpcomingCount     = await _context.Consultations
            .CountAsync(c => c.AppointmentDate >= DateTime.Today
                          && c.Status == ConsultationStatus.Scheduled);

        return View();
    }

    // GET /Dashboard/Patients  — liste paginée des patients (5 par page)
    public async Task<IActionResult> Patients(int page = 1)
    {
        const int pageSize = 5;

        var query = _context.Patients
            .AsNoTracking()
            .Include(p => p.Consultations)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var patients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages  = totalPages;
        ViewBag.TotalCount  = total;

        return View(patients);
    }

    // GET /Dashboard/PatientDetail/5  — fiche patient complète
    public async Task<IActionResult> PatientDetail(int id)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Include(p => p.Pathologies)
            .Include(p => p.Consultations.OrderByDescending(c => c.AppointmentDate))
                .ThenInclude(c => c.Doctor)
                    .ThenInclude(d => d!.Department)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound();

        return View(patient);
    }

    // GET /Dashboard/Doctors  — liste paginée des médecins (5 par page)
    public async Task<IActionResult> Doctors(int page = 1)
    {
        const int pageSize = 5;

        var query = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Include(d => d.Consultations)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var doctors = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages  = totalPages;
        ViewBag.TotalCount  = total;

        return View(doctors);
    }

    // GET /Dashboard/DoctorDetail/5  — planning médecin (consultations à venir)
    public async Task<IActionResult> DoctorDetail(int id)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Include(d => d.Consultations
                .Where(c => c.AppointmentDate >= DateTime.Today
                         && c.Status != ConsultationStatus.Cancelled)
                .OrderBy(c => c.AppointmentDate))
                .ThenInclude(c => c.Patient)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
            return NotFound();

        return View(doctor);
    }

    // GET /Dashboard/DepartmentStats  — statistiques par département
    public async Task<IActionResult> DepartmentStats()
    {
        var stats = await _context.Departments
            .AsNoTracking()
            .Select(dep => new DepartmentStatViewModel
            {
                DepartmentId   = dep.Id,
                DepartmentName = dep.Name,
                HeadDoctorName = dep.HeadDoctor != null
                    ? dep.HeadDoctor.FirstName + " " + dep.HeadDoctor.LastName
                    : "—",
                DoctorCount       = dep.Doctors.Count,
                ConsultationCount = dep.Doctors.SelectMany(d => d.Consultations).Count(),
                UpcomingCount     = dep.Doctors
                    .SelectMany(d => d.Consultations)
                    .Count(c => c.AppointmentDate >= DateTime.Today
                             && c.Status == ConsultationStatus.Scheduled)
            })
            .OrderBy(s => s.DepartmentName)
            .ToListAsync();

        return View(stats);
    }

    // GET /Dashboard/UpcomingConsultations — toutes les consultations planifiées
    public async Task<IActionResult> UpcomingConsultations()
    {
        var consultations = await _context.Consultations
            .AsNoTracking()
            .Where(c => c.AppointmentDate >= DateTime.Today
                     && c.Status == ConsultationStatus.Scheduled)
            .Include(c => c.Patient)
            .Include(c => c.Doctor)
                .ThenInclude(d => d!.Department)
            .OrderBy(c => c.AppointmentDate)
            .ToListAsync();

        return View(consultations);
    }
}
