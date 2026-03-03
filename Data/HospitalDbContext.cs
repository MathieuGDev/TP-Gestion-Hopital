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
    }

}
