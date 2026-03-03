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
    }

}
