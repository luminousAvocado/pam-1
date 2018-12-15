using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PAM.Models;

namespace PAM.Data
{
    public class AppDbContext : DbContext
    {
        private IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Bureau> Bureaus { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Models.System> Systems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.System>().HasQueryFilter(s => !s.Retired);
            modelBuilder.Entity<UnitSystem>().HasKey(x => new { x.UnitId, x.SystemId });

            modelBuilder.Entity<Request>().HasOne(r => r.RequestedBy).WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Request>().HasOne(r => r.RequestedFor).WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Request>().Property(r => r.RequestStatus).HasConversion(
                v => v.ToString(),
                v => (RequestStatus)Enum.Parse(typeof(RequestStatus), v));
            modelBuilder.Entity<Request>().Property(r => r.CaseloadType).HasConversion(
                v => v.ToString(),
                v => (CaseloadType)Enum.Parse(typeof(CaseloadType), v));
            modelBuilder.Entity<Request>().Property(r => r.CaseloadFunction).HasConversion(
                v => v.ToString(),
                v => (CaseloadFunction)Enum.Parse(typeof(CaseloadFunction), v));
            modelBuilder.Entity<Request>().Property(r => r.DepartureReason).HasConversion(
                v => v.ToString(),
                v => (DepartureReason)Enum.Parse(typeof(DepartureReason), v));

            modelBuilder.Entity<RequestedSystem>().HasKey(x => new { x.RequestId, x.SystemId });

            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Username);
            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.EmployeeNumber);
            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Email);
            modelBuilder.Entity<Employee>().HasData(new Employee()
            {
                EmployeeId = 1,
                Username = _configuration.GetSection("Presets")["AdminUser"],
                EmployeeNumber = "1111",
                Email = "admin@localhost.localdomain",
                FirstName = "PAM",
                LastName = "Admin",
                IsAdmin = true
            });

            modelBuilder.Entity<SystemAccess>().HasKey(x => new { x.EmployeeId, x.SystemId });
            modelBuilder.Entity<SystemAccess>().Property(r => r.SystemAccessStatus).HasConversion(
                v => v.ToString(),
                v => (SystemAccessStatus)Enum.Parse(typeof(SystemAccessStatus), v));
        }
    }

}
