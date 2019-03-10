using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAM.Data;

namespace PAM.Test.Data
{
    public class DataServiceFixture
    {
        public DataServiceFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(Configuration.GetConnectionString("UnitTestConnection"))
                .Options;

            ServiceProvider = new ServiceCollection()
                .AddLogging(config => config.AddConsole())
                .AddAutoMapper()
                .BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }

        public DbContextOptions<AppDbContext> DbContextOptions { get; }

        public IServiceProvider ServiceProvider { get; }

        public ILoggerFactory LoggerFactory => ServiceProvider.GetService<ILoggerFactory>();
    }
}
