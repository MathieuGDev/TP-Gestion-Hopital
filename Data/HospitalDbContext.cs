using System;
using Microsoft.EntityFrameworkCore;

namespace tp_hospital.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Patient>           Patients          { get; set; }
    public DbSet<Models.Department>        Departments       { get; set; }
    public DbSet<Models.Doctor>            Doctors           { get; set; }
    public DbSet<Models.Consultation>      Consultations     { get; set; }
    public DbSet<Models.MedicalStaff>      MedicalStaff      { get; set; }

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


        modelBuilder.Entity<Models.Patient>()
            .OwnsOne(p => p.Address, a =>
            {
                a.Property(x => x.Street)     .HasColumnName("Address_Street");
                a.Property(x => x.City)       .HasColumnName("Address_City");
                a.Property(x => x.PostalCode) .HasColumnName("Address_PostalCode");
                a.Property(x => x.Country)    .HasColumnName("Address_Country");

                a.HasData(
                    new { PatientId = 1, Street = "12 rue de la Paix",       City = "Paris",     PostalCode = "75001", Country = "France" },
                    new { PatientId = 2, Street = "3 avenue des Sciences",   City = "Lyon",      PostalCode = "69001", Country = "France" },
                    new { PatientId = 3, Street = "8 boulevard Victor Hugo", City = "Marseille", PostalCode = "13001", Country = "France" }
                );
            });

        modelBuilder.Entity<Models.Department>()
            .OwnsOne(d => d.ContactAddress, a =>
            {
                a.Property(x => x.Street)     .HasColumnName("ContactAddress_Street");
                a.Property(x => x.City)       .HasColumnName("ContactAddress_City");
                a.Property(x => x.PostalCode) .HasColumnName("ContactAddress_PostalCode");
                a.Property(x => x.Country)    .HasColumnName("ContactAddress_Country");

                a.HasData(
                    new { DepartmentId = 1, Street = "1 rue du Coeur",       City = "Paris",     PostalCode = "75001", Country = "France" },
                    new { DepartmentId = 2, Street = "2 avenue des Urgences",City = "Paris",     PostalCode = "75002", Country = "France" },
                    new { DepartmentId = 3, Street = "3 rue des Enfants",    City = "Paris",     PostalCode = "75003", Country = "France" },
                    new { DepartmentId = 4, Street = "",                     City = "",          PostalCode = "",      Country = "France" },
                    new { DepartmentId = 5, Street = "",                     City = "",          PostalCode = "",      Country = "France" }
                );
            });

        modelBuilder.Entity<Models.Department>()
            .HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Models.Doctor>()
            .HasOne(d => d.Department)
            .WithMany(dep => dep.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Department>()
            .HasOne(dep => dep.HeadDoctor)
            .WithMany()
            .HasForeignKey(dep => dep.HeadDoctorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Models.Consultation>()
            .HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Consultation>()
            .HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Consultation>()
            .HasIndex(c => new { c.PatientId, c.DoctorId, c.AppointmentDate })
            .IsUnique();


        modelBuilder.Entity<Models.MedicalStaff>()
            .HasDiscriminator<string>("StaffType")
            .HasValue<Models.MedicalDoctor>("Doctor")
            .HasValue<Models.Nurse>("Nurse")
            .HasValue<Models.AdministrativeStaff>("Admin");

        modelBuilder.Entity<Models.MedicalDoctor>()
            .Property(d => d.Specialty).HasMaxLength(100);
        modelBuilder.Entity<Models.MedicalDoctor>()
            .HasIndex(d => d.LicenseNumber).IsUnique();
        modelBuilder.Entity<Models.Nurse>()
            .Property(n => n.Grade).HasMaxLength(50);
        modelBuilder.Entity<Models.AdministrativeStaff>()
            .Property(a => a.Function).HasMaxLength(100);

        // Seeders

        modelBuilder.Entity<Models.Department>().HasData(
            new Models.Department { Id = 1, HeadDoctorId = 1, Name = "Cardiologie" },
            new Models.Department { Id = 2, HeadDoctorId = 2, Name = "Urgences"    },
            new Models.Department { Id = 3, HeadDoctorId = 3, Name = "Pediatrie"   },
            // Sous-départements
            new Models.Department { Id = 4, Name = "Cardiologie adulte",       ParentDepartmentId = 1 },
            new Models.Department { Id = 5, Name = "Cardiologie pediatrique",  ParentDepartmentId = 1 }
        );

        modelBuilder.Entity<Models.Doctor>().HasData(
            new Models.Doctor { Id = 1, FirstName = "Sophie", LastName = "Martin",  Specialty = "Cardiologie",       LicenseNumber = "LIC-0001", DepartmentId = 1 },
            new Models.Doctor { Id = 2, FirstName = "Lucas",  LastName = "Bernard", Specialty = "Medecine generale", LicenseNumber = "LIC-0002", DepartmentId = 2 },
            new Models.Doctor { Id = 3, FirstName = "Emma",   LastName = "Dubois",  Specialty = "Pediatrie",         LicenseNumber = "LIC-0003", DepartmentId = 3 }
        );

        modelBuilder.Entity<Models.Patient>().HasData(
            new Models.Patient { Id = 1, FolderNumber = 1001, FirstName = "Jean",   LastName = "Dupont", DateOfBirth = new DateTime(1985, 4,  12), Email = "jean.dupont@email.com"  },
            new Models.Patient { Id = 2, FolderNumber = 1002, FirstName = "Marie",  LastName = "Curie",  DateOfBirth = new DateTime(1990, 11, 7),  Email = "marie.curie@email.com"  },
            new Models.Patient { Id = 3, FolderNumber = 1003, FirstName = "Thomas", LastName = "Leroy",  DateOfBirth = new DateTime(2000, 6,  25), Email = "thomas.leroy@email.com" }
        );

        modelBuilder.Entity<Models.Consultation>().HasData(
            new Models.Consultation { Id = 1, PatientId = 1, DoctorId = 1, AppointmentDate = new DateTime(2026, 2, 21, 10, 0, 0), Status = Models.ConsultationStatus.Completed, Notes = "Bilan cardiaque annuel, RAS."   },
            new Models.Consultation { Id = 2, PatientId = 2, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 3,  10, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Consultation fievre."           },
            new Models.Consultation { Id = 3, PatientId = 3, DoctorId = 3, AppointmentDate = new DateTime(2026, 3, 8,  14, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Suivi pediatrique."             },
            new Models.Consultation { Id = 4, PatientId = 1, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 6,  9,  0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Controle tension arterielle."   }
        );

        // Seed personnel
        modelBuilder.Entity<Models.MedicalDoctor>().HasData(
            new Models.MedicalDoctor { Id = 1, FirstName = "Sophie", LastName = "Martin",  Specialty = "Cardiologie",       LicenseNumber = "EMP-0001", HireDate = new DateTime(2015, 9, 1),  Salary = 6500m },
            new Models.MedicalDoctor { Id = 2, FirstName = "Lucas",  LastName = "Bernard", Specialty = "Medecine generale", LicenseNumber = "EMP-0002", HireDate = new DateTime(2018, 3, 15), Salary = 6200m },
            new Models.MedicalDoctor { Id = 3, FirstName = "Emma",   LastName = "Dubois",  Specialty = "Pediatrie",         LicenseNumber = "EMP-0003", HireDate = new DateTime(2020, 1, 10), Salary = 6300m }
        );
        modelBuilder.Entity<Models.Nurse>().HasData(
            new Models.Nurse { Id = 4, FirstName = "Claire", LastName = "Morel",  Service = "Cardiologie", Grade = "IDE",   HireDate = new DateTime(2019, 6, 1),  Salary = 2800m },
            new Models.Nurse { Id = 5, FirstName = "Pierre", LastName = "Renard", Service = "Urgences",    Grade = "IADE",  HireDate = new DateTime(2021, 9, 1),  Salary = 3100m }
        );
        modelBuilder.Entity<Models.AdministrativeStaff>().HasData(
            new Models.AdministrativeStaff { Id = 6, FirstName = "Nathalie", LastName = "Petit", Function = "Secretaire medicale", HireDate = new DateTime(2017, 4, 3), Salary = 2200m }
        );
    }
}

