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
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Department>()
            .HasOne(dep => dep.HeadDoctor)
            .WithMany()
            .HasForeignKey(dep => dep.HeadDoctorId)
            .OnDelete(DeleteBehavior.NoAction);
    }

}
