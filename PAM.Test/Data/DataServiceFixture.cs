using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAM.Data;

namespace PAM.Test.Data
{
    public class DataServiceFixture : IDisposable
    {
        public DataServiceFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(Configuration.GetConnectionString("UnitTestConnection"))
                .Options;

            var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.AddConsole())
                .BuildServiceProvider();

            LoggerFactory = serviceProvider.GetService<ILoggerFactory>();
        }

        public IConfiguration Configuration { get; }

        public DbContextOptions<AppDbContext> DbContextOptions { get; }

        public ILoggerFactory LoggerFactory { get; }

        public void Dispose()
        {
            LoggerFactory.Dispose();
        }
    }
}
