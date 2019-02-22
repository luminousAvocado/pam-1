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
        public DbSet<BureauType> BureauTypes { get; set; }
        public DbSet<Bureau> Bureaus { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Models.System> Systems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Requester> Requesters { get; set; }
        public DbSet<SystemAccess> SystemAccesses { get; set; }
        public DbSet<UnitSystem> UnitSystems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>().Property(l => l.Deleted).HasDefaultValue(false);
            modelBuilder.Entity<Location>().HasQueryFilter(l => !l.Deleted);

            modelBuilder.Entity<Bureau>().Property(b => b.DisplayOrder).HasDefaultValue(50);
            modelBuilder.Entity<Bureau>().Property(b => b.Deleted).HasDefaultValue(false);
            modelBuilder.Entity<Bureau>().HasQueryFilter(b => !b.Deleted);

            modelBuilder.Entity<Unit>().Property(u => u.Deleted).HasDefaultValue(false);
            modelBuilder.Entity<Unit>().HasQueryFilter(u => !u.Deleted);

            modelBuilder.Entity<Models.System>().Property(s => s.Retired).HasDefaultValue(false);
            modelBuilder.Entity<Models.System>().HasQueryFilter(s => !s.Retired);
            modelBuilder.Entity<UnitSystem>().HasKey(x => new { x.UnitId, x.SystemId });

            modelBuilder.Entity<Request>().HasQueryFilter(r => !r.Deleted);
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

            modelBuilder.Entity<RequestedSystem>().HasKey(s => new { s.RequestId, s.SystemId });
            modelBuilder.Entity<RequestedSystem>().Property(s => s.AccessType).HasConversion(
                v => v.ToString(),
                v => (SystemAccessType)Enum.Parse(typeof(SystemAccessType), v));

            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Username);
            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Email);
            modelBuilder.Entity<Employee>().HasData(new Employee()
            {
                EmployeeId = 1,
                Username = _configuration.GetSection("Presets")["AdminUser"],
                Name = "Pam Admin (e111111)",
                Email = "pam@localhost.localdomain",
                FirstName = "Pam",
                LastName = "Admin",
                IsAdmin = true
            });

            modelBuilder.Entity<SystemAccess>().HasAlternateKey(s => new { s.RequestId, s.SystemId });
            modelBuilder.Entity<SystemAccess>().Property(s => s.AccessType).HasConversion(
                v => v.ToString(),
                v => (SystemAccessType)Enum.Parse(typeof(SystemAccessType), v));
        }
    }
}
