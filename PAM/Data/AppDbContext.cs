using Microsoft.EntityFrameworkCore;
using PAM.Models;

namespace PAM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Bureau> Bureaus { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Models.System> Systems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.System>().HasQueryFilter(s => !s.Retired);
            modelBuilder.Entity<UnitSystem>().HasKey(x => new { x.UnitId, x.SystemId });
        }
    }

}
