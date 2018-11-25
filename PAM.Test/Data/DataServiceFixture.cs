using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PAM.Data;

namespace PAM.Test.Data
{
    public class DataServiceFixture
    {
        public DataServiceFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(configuration.GetConnectionString("UnitTestConnection"))
                .Options;
        }

        public DbContextOptions<AppDbContext> DbContextOptions { get; }

    }
}
