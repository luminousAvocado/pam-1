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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.System>().HasQueryFilter(s => !s.Retired);
            modelBuilder.Entity<UnitSystem>().HasKey(x => new { x.UnitId, x.SystemId });

            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Username);
            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.EmployeeNumber);
            modelBuilder.Entity<Employee>().HasAlternateKey(e => e.Email);
            modelBuilder.Entity<Employee>().HasData(new Employee()
            {
                EmployeeId = 1,
                Username = _configuration.GetSection("Presets")["AdminUser"],
                EmployeeNumber = "1234",
                Email = "admin@localhost.localdomain",
                FirstName = "PAM",
                LastName = "Admin",
                IsAdmin = true
            });
        }
    }

}
