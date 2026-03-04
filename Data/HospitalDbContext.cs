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
                    new { PatientId =  1, Street = "12 rue de la Paix",          City = "Paris",         PostalCode = "75001", Country = "France" },
                    new { PatientId =  2, Street = "3 avenue des Sciences",     City = "Lyon",          PostalCode = "69001", Country = "France" },
                    new { PatientId =  3, Street = "8 boulevard Victor Hugo",   City = "Marseille",     PostalCode = "13001", Country = "France" },
                    new { PatientId =  4, Street = "5 rue des Lilas",           City = "Bordeaux",      PostalCode = "33000", Country = "France" },
                    new { PatientId =  5, Street = "22 avenue Foch",            City = "Toulouse",      PostalCode = "31000", Country = "France" },
                    new { PatientId =  6, Street = "14 rue Nationale",          City = "Lille",         PostalCode = "59000", Country = "France" },
                    new { PatientId =  7, Street = "7 impasse des Roses",       City = "Nantes",        PostalCode = "44000", Country = "France" },
                    new { PatientId =  8, Street = "30 rue du Moulin",          City = "Strasbourg",    PostalCode = "67000", Country = "France" },
                    new { PatientId =  9, Street = "18 chemin des Vignes",      City = "Montpellier",   PostalCode = "34000", Country = "France" },
                    new { PatientId = 10, Street = "2 place de la Republique",  City = "Rennes",        PostalCode = "35000", Country = "France" },
                    new { PatientId = 11, Street = "9 rue Saint-Jacques",       City = "Nice",          PostalCode = "06000", Country = "France" },
                    new { PatientId = 12, Street = "45 boulevard Gambetta",     City = "Grenoble",      PostalCode = "38000", Country = "France" },
                    new { PatientId = 13, Street = "3 allee des Peupliers",     City = "Dijon",         PostalCode = "21000", Country = "France" },
                    new { PatientId = 14, Street = "11 rue du Four",            City = "Reims",         PostalCode = "51100", Country = "France" },
                    new { PatientId = 15, Street = "60 avenue Jean Jaures",     City = "Clermont-Fd",   PostalCode = "63000", Country = "France" },
                    new { PatientId = 16, Street = "28 rue des Capucines",      City = "Rouen",         PostalCode = "76000", Country = "France" },
                    new { PatientId = 17, Street = "6 place du Capitole",       City = "Toulouse",      PostalCode = "31000", Country = "France" },
                    new { PatientId = 18, Street = "15 rue Pasteur",            City = "Metz",          PostalCode = "57000", Country = "France" },
                    new { PatientId = 19, Street = "33 avenue de la Gare",      City = "Tours",         PostalCode = "37000", Country = "France" },
                    new { PatientId = 20, Street = "8 rue des Hirondelles",     City = "Caen",          PostalCode = "14000", Country = "France" }
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
            new Models.Doctor { Id =  1, FirstName = "Sophie",   LastName = "Martin",    Specialty = "Cardiologie",       LicenseNumber = "LIC-0001", DepartmentId = 1 },
            new Models.Doctor { Id =  2, FirstName = "Lucas",    LastName = "Bernard",   Specialty = "Medecine generale", LicenseNumber = "LIC-0002", DepartmentId = 2 },
            new Models.Doctor { Id =  3, FirstName = "Emma",     LastName = "Dubois",    Specialty = "Pediatrie",         LicenseNumber = "LIC-0003", DepartmentId = 3 },
            new Models.Doctor { Id =  4, FirstName = "Paul",     LastName = "Lefebvre",  Specialty = "Cardiologie",       LicenseNumber = "LIC-0004", DepartmentId = 1 },
            new Models.Doctor { Id =  5, FirstName = "Julie",    LastName = "Moreau",    Specialty = "Urgences",          LicenseNumber = "LIC-0005", DepartmentId = 2 },
            new Models.Doctor { Id =  6, FirstName = "Antoine",  LastName = "Girard",    Specialty = "Pediatrie",         LicenseNumber = "LIC-0006", DepartmentId = 3 },
            new Models.Doctor { Id =  7, FirstName = "Camille",  LastName = "Laurent",   Specialty = "Cardiologie",       LicenseNumber = "LIC-0007", DepartmentId = 1 },
            new Models.Doctor { Id =  8, FirstName = "Maxime",   LastName = "Fontaine",  Specialty = "Urgences",          LicenseNumber = "LIC-0008", DepartmentId = 2 },
            new Models.Doctor { Id =  9, FirstName = "Isabelle", LastName = "Roux",      Specialty = "Pediatrie",         LicenseNumber = "LIC-0009", DepartmentId = 3 },
            new Models.Doctor { Id = 10, FirstName = "Nicolas",  LastName = "Mercier",   Specialty = "Cardiologie",       LicenseNumber = "LIC-0010", DepartmentId = 1 }
        );

        modelBuilder.Entity<Models.Patient>().HasData(
            new Models.Patient { Id =  1, FolderNumber = 1001, FirstName = "Jean",      LastName = "Dupont",    DateOfBirth = new DateTime(1985,  4, 12), Email = "jean.dupont@email.com"      },
            new Models.Patient { Id =  2, FolderNumber = 1002, FirstName = "Marie",     LastName = "Curie",     DateOfBirth = new DateTime(1990, 11,  7), Email = "marie.curie@email.com"      },
            new Models.Patient { Id =  3, FolderNumber = 1003, FirstName = "Thomas",    LastName = "Leroy",     DateOfBirth = new DateTime(2000,  6, 25), Email = "thomas.leroy@email.com"     },
            new Models.Patient { Id =  4, FolderNumber = 1004, FirstName = "Alice",     LastName = "Bernot",    DateOfBirth = new DateTime(1978,  3, 14), Email = "alice.bernot@email.com"     },
            new Models.Patient { Id =  5, FolderNumber = 1005, FirstName = "Pierre",    LastName = "Faure",     DateOfBirth = new DateTime(1965,  8, 22), Email = "pierre.faure@email.com"     },
            new Models.Patient { Id =  6, FolderNumber = 1006, FirstName = "Lucie",     LastName = "Garnier",   DateOfBirth = new DateTime(1995,  1, 30), Email = "lucie.garnier@email.com"    },
            new Models.Patient { Id =  7, FolderNumber = 1007, FirstName = "Marc",      LastName = "Simon",     DateOfBirth = new DateTime(1982,  9,  5), Email = "marc.simon@email.com"       },
            new Models.Patient { Id =  8, FolderNumber = 1008, FirstName = "Isabelle",  LastName = "Henry",     DateOfBirth = new DateTime(1971, 12, 18), Email = "isabelle.henry@email.com"   },
            new Models.Patient { Id =  9, FolderNumber = 1009, FirstName = "David",     LastName = "Blanc",     DateOfBirth = new DateTime(2003,  7, 11), Email = "david.blanc@email.com"      },
            new Models.Patient { Id = 10, FolderNumber = 1010, FirstName = "Catherine", LastName = "Richard",   DateOfBirth = new DateTime(1988,  2, 28), Email = "catherine.richard@email.com" },
            new Models.Patient { Id = 11, FolderNumber = 1011, FirstName = "Nicolas",   LastName = "Lambert",   DateOfBirth = new DateTime(1993,  5, 16), Email = "nicolas.lambert@email.com"  },
            new Models.Patient { Id = 12, FolderNumber = 1012, FirstName = "Sophie",    LastName = "Dumont",    DateOfBirth = new DateTime(1979, 10,  3), Email = "sophie.dumont@email.com"    },
            new Models.Patient { Id = 13, FolderNumber = 1013, FirstName = "Julien",    LastName = "Dupuis",    DateOfBirth = new DateTime(2008,  4, 20), Email = "julien.dupuis@email.com"    },
            new Models.Patient { Id = 14, FolderNumber = 1014, FirstName = "Claire",    LastName = "Bertrand",  DateOfBirth = new DateTime(1960,  6,  7), Email = "claire.bertrand@email.com"  },
            new Models.Patient { Id = 15, FolderNumber = 1015, FirstName = "Antoine",   LastName = "Morin",     DateOfBirth = new DateTime(1975, 11, 25), Email = "antoine.morin@email.com"    },
            new Models.Patient { Id = 16, FolderNumber = 1016, FirstName = "Marine",    LastName = "Rousseau",  DateOfBirth = new DateTime(1998,  8, 14), Email = "marine.rousseau@email.com"  },
            new Models.Patient { Id = 17, FolderNumber = 1017, FirstName = "Francois",  LastName = "Legrand",   DateOfBirth = new DateTime(1956,  3,  9), Email = "francois.legrand@email.com" },
            new Models.Patient { Id = 18, FolderNumber = 1018, FirstName = "Laura",     LastName = "Petit",     DateOfBirth = new DateTime(2001, 12,  1), Email = "laura.petit@email.com"      },
            new Models.Patient { Id = 19, FolderNumber = 1019, FirstName = "Romain",    LastName = "Chevalier", DateOfBirth = new DateTime(1987,  7, 19), Email = "romain.chevalier@email.com" },
            new Models.Patient { Id = 20, FolderNumber = 1020, FirstName = "Nadia",     LastName = "Bonnet",    DateOfBirth = new DateTime(1992,  4, 23), Email = "nadia.bonnet@email.com"     }
        );

        modelBuilder.Entity<Models.Consultation>().HasData(
            new Models.Consultation { Id = 1, PatientId = 1, DoctorId = 1, AppointmentDate = new DateTime(2026, 2, 21, 10, 0, 0), Status = Models.ConsultationStatus.Completed, Notes = "Bilan cardiaque annuel, RAS."   },
            new Models.Consultation { Id = 2, PatientId = 2, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 3,  10, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Consultation fievre."           },
            new Models.Consultation { Id = 3, PatientId = 3, DoctorId = 3, AppointmentDate = new DateTime(2026, 3, 8,  14, 0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Suivi pediatrique."             },
            new Models.Consultation { Id = 4, PatientId = 1, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 6,  9,  0, 0), Status = Models.ConsultationStatus.Scheduled, Notes = "Controle tension arterielle."   }
        );

        // Seed personnel
        modelBuilder.Entity<Models.MedicalDoctor>().HasData(
            new Models.MedicalDoctor { Id =  1, FirstName = "Sophie",   LastName = "Martin",   Specialty = "Cardiologie",       LicenseNumber = "EMP-0001", HireDate = new DateTime(2015,  9,  1), Salary = 6500m },
            new Models.MedicalDoctor { Id =  2, FirstName = "Lucas",    LastName = "Bernard",  Specialty = "Medecine generale", LicenseNumber = "EMP-0002", HireDate = new DateTime(2018,  3, 15), Salary = 6200m },
            new Models.MedicalDoctor { Id =  3, FirstName = "Emma",     LastName = "Dubois",   Specialty = "Pediatrie",         LicenseNumber = "EMP-0003", HireDate = new DateTime(2020,  1, 10), Salary = 6300m },
            new Models.MedicalDoctor { Id =  7, FirstName = "Paul",     LastName = "Lefebvre", Specialty = "Cardiologie",       LicenseNumber = "EMP-0004", HireDate = new DateTime(2016,  5, 20), Salary = 6400m },
            new Models.MedicalDoctor { Id =  8, FirstName = "Julie",    LastName = "Moreau",   Specialty = "Urgences",          LicenseNumber = "EMP-0005", HireDate = new DateTime(2019,  7,  1), Salary = 6100m },
            new Models.MedicalDoctor { Id =  9, FirstName = "Antoine",  LastName = "Girard",   Specialty = "Pediatrie",         LicenseNumber = "EMP-0006", HireDate = new DateTime(2021,  2, 14), Salary = 6000m },
            new Models.MedicalDoctor { Id = 10, FirstName = "Camille",  LastName = "Laurent",  Specialty = "Cardiologie",       LicenseNumber = "EMP-0007", HireDate = new DateTime(2017, 11,  3), Salary = 6350m },
            new Models.MedicalDoctor { Id = 11, FirstName = "Maxime",   LastName = "Fontaine", Specialty = "Urgences",          LicenseNumber = "EMP-0008", HireDate = new DateTime(2022,  4, 18), Salary = 5900m },
            new Models.MedicalDoctor { Id = 12, FirstName = "Isabelle", LastName = "Roux",     Specialty = "Pediatrie",         LicenseNumber = "EMP-0009", HireDate = new DateTime(2019, 10,  5), Salary = 6150m },
            new Models.MedicalDoctor { Id = 13, FirstName = "Nicolas",  LastName = "Mercier",  Specialty = "Cardiologie",       LicenseNumber = "EMP-0010", HireDate = new DateTime(2014,  8, 22), Salary = 6700m }
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

