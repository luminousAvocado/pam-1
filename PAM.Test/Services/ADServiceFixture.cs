using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAM.Services;

namespace PAM.Test.Services
{
    public class ADServiceFixture : IDisposable
    {
        public ADServiceFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            LoggerFactory = new LoggerFactory();

            ADService = new ADService(Configuration, LoggerFactory.CreateLogger<ADService>());
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IADService ADService { get; }

        public void Dispose()
        {
            LoggerFactory.Dispose();
        }
    }
}
