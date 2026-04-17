using Microsoft.EntityFrameworkCore;
using SmartCareApp.Models;

namespace SmartCareApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<AdminAccess> AdminAccessCodes => Set<AdminAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(patient => patient.FullName).HasMaxLength(120);
            entity.Property(patient => patient.NationalId).HasMaxLength(11);
            entity.Property(patient => patient.PrimarySymptom).HasMaxLength(160);
            entity.Property(patient => patient.ChronicConditions).HasMaxLength(300);
            entity.Property(patient => patient.IntakeNotes).HasMaxLength(500);
            entity.Property(patient => patient.AccessCode).HasMaxLength(20);
            entity.Property(patient => patient.ProfileImageFileName).HasMaxLength(120);
            entity.Property(patient => patient.CareStatus).HasConversion<string>().HasMaxLength(40);

            entity.HasIndex(patient => patient.NationalId).IsUnique();
            entity.HasIndex(patient => patient.AccessCode).IsUnique();

            entity.HasOne(patient => patient.CreatedByDoctor)
                .WithMany(doctor => doctor.CreatedPatients)
                .HasForeignKey(patient => patient.CreatedByDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(doctor => doctor.FullName).HasMaxLength(120);
            entity.Property(doctor => doctor.Specialty).HasMaxLength(80);
            entity.Property(doctor => doctor.RoomNumber).HasMaxLength(40);
            entity.Property(doctor => doctor.Username).HasMaxLength(40);
            entity.Property(doctor => doctor.Password).HasMaxLength(80);
            entity.Property(doctor => doctor.ProfileImageFileName).HasMaxLength(120);

            entity.HasIndex(doctor => doctor.Username).IsUnique();
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(appointment => appointment.Reason).HasMaxLength(200);
            entity.Property(appointment => appointment.VisitType).HasMaxLength(60);
            entity.Property(appointment => appointment.Status).HasConversion<string>().HasMaxLength(40);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.Property(history => history.Category).HasMaxLength(60);
            entity.Property(history => history.Title).HasMaxLength(120);
        });

        modelBuilder.Entity<AdminAccess>(entity =>
        {
            entity.Property(access => access.AccessCode).HasMaxLength(20);
        });
    }
}
