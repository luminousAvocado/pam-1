using Microsoft.EntityFrameworkCore;
using PAM.Models;

namespace PAM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bureau> Bureaus { get; set; }
        public DbSet<BureauType> BureauTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }

}
