using Microsoft.EntityFrameworkCore;
using ClinicBooking.Models;
using Microsoft.Extensions.Configuration;

namespace ClinicBooking.Data
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(
            DbContextOptions<ClinicContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration =
                    new ConfigurationBuilder()
                        .SetBasePath(
                            Directory.GetCurrentDirectory())
                        .AddJsonFile(
                            "appsettings.json",
                            optional: false,
                            reloadOnChange: true)
                        .Build();

                var connectionString =
                    configuration.GetConnectionString(
                        "DefaultConnection");

                optionsBuilder.UseSqlServer(
                    connectionString);
            }
        }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // =====================================
            // APPOINTMENT -> DOCTOR RELATIONSHIP
            // =====================================

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================
            // UNIQUE CONSTRAINTS
            // =====================================

            // One licence number per doctor
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenceNumber)
                .IsUnique();


            // One email per user
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // One appointment slot per doctor
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new
                {
                    a.DoctorId,
                    a.SlotDateTime
                })
                .IsUnique();
        }
    }
}