using System;
using Microsoft.EntityFrameworkCore;

namespace tp_hospital.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Patient> Patients { get; set; }
    public DbSet<Models.Department> Departments { get; set; }
    public DbSet<Models.Doctor> Doctors { get; set; }
    public DbSet<Models.Consultation> Consultations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Patient>()
            .HasIndex(p => p.Email)
            .IsUnique();
     
        modelBuilder.Entity<Models.Patient>()
            .HasIndex(p => p.FolderNumber)
            .IsUnique();

        modelBuilder.Entity<Models.Department>()
            .HasIndex(d => d.Name)
            .IsUnique();

        modelBuilder.Entity<Models.Doctor>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        modelBuilder.Entity<Models.Doctor>()
            .HasOne(d => d.Department)
            .WithMany(dep => dep.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict); // Un médecin ne peut pas être supprimé si il est référencé par un département

        modelBuilder.Entity<Models.Department>()
            .HasOne(dep => dep.HeadDoctor)
            .WithMany()
            .HasForeignKey(dep => dep.HeadDoctorId)
            .OnDelete(DeleteBehavior.NoAction); // Un département ne peut pas être supprimé si il est référencé par un médecin en tant que chef de département

        // Relation Consultation -> Patient (un patient peut avoir plusieurs consultations)
        modelBuilder.Entity<Models.Consultation>()
            .HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation Consultation -> Doctor (un médecin peut avoir plusieurs consultations)
        modelBuilder.Entity<Models.Consultation>()
            .HasOne(c => c.Doctor)
            .WithMany()
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contrainte : un patient ne peut pas avoir deux consultations au même moment avec le même médecin
        modelBuilder.Entity<Models.Consultation>()
            .HasIndex(c => new { c.PatientId, c.DoctorId, c.AppointmentDate })
            .IsUnique();

        // Seeders

        modelBuilder.Entity<Models.Department>().HasData(
            new Models.Department { Id = 1, Name = "Cardiologie" },
            new Models.Department { Id = 2, Name = "Urgences" },
            new Models.Department { Id = 3, Name = "Pediatrie" }
        );

        modelBuilder.Entity<Models.Doctor>().HasData(
            new Models.Doctor { Id = 1, FirstName = "Sophie",  LastName = "Martin",  Specialty = "Cardiologie",       LicenseNumber = "LIC-0001", DepartmentId = 1 },
            new Models.Doctor { Id = 2, FirstName = "Lucas",   LastName = "Bernard", Specialty = "Medecine generale", LicenseNumber = "LIC-0002", DepartmentId = 2 },
            new Models.Doctor { Id = 3, FirstName = "Emma",    LastName = "Dubois",  Specialty = "Pediatrie",         LicenseNumber = "LIC-0003", DepartmentId = 3 }
        );

        modelBuilder.Entity<Models.Patient>().HasData(
            new Models.Patient { Id = 1, FolderNumber = 1001, FirstName = "Jean",   LastName = "Dupont", DateOfBirth = new DateTime(1985, 4,  12), Address = "12 rue de la Paix, Paris",           Email = "jean.dupont@email.com"  },
            new Models.Patient { Id = 2, FolderNumber = 1002, FirstName = "Marie",  LastName = "Curie",  DateOfBirth = new DateTime(1990, 11, 7),  Address = "3 avenue des Sciences, Lyon",         Email = "marie.curie@email.com"  },
            new Models.Patient { Id = 3, FolderNumber = 1003, FirstName = "Thomas", LastName = "Leroy",  DateOfBirth = new DateTime(2000, 6,  25), Address = "8 boulevard Victor Hugo, Marseille",  Email = "thomas.leroy@email.com" }
        );

        modelBuilder.Entity<Models.Consultation>().HasData(
            new Models.Consultation { Id = 1, PatientId = 1, DoctorId = 1, AppointmentDate = new DateTime(2026, 2, 21, 10, 0, 0), Status = Models.ConsultationStatus.Completed, Notes = "Bilan cardiaque annuel, RAS."    },
            new Models.Consultation { Id = 2, PatientId = 2, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 3,  10, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Consultation fievre."            },
            new Models.Consultation { Id = 3, PatientId = 3, DoctorId = 3, AppointmentDate = new DateTime(2026, 3, 8,  14, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Suivi pediatrique."              },
            new Models.Consultation { Id = 4, PatientId = 1, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 6,  9,  0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Controle tension arterielle."    }
        );
    }

}
