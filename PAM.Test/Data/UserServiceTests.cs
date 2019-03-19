using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAM.Data;
using Xunit;

namespace PAM.Test.Data
{
    [Collection(nameof(DataServiceCollection))]
    public class UserServiceTests
    {
        readonly IConfiguration _configuration;
        readonly DbContextOptions<AppDbContext> _dbContextOptions;
        readonly IMapper _mapper;
        readonly ILogger<UserService> _logger;

        public UserServiceTests(DataServiceFixture dataServiceFixture)
        {
            _configuration = dataServiceFixture.Configuration;
            _dbContextOptions = dataServiceFixture.DbContextOptions;
            _mapper = dataServiceFixture.ServiceProvider.GetService<IMapper>();
            _logger = dataServiceFixture.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<UserService>();
        }

        [Fact]
        public void GetAdminTest()
        {
            using (var dbContext = new AppDbContext(_dbContextOptions, _configuration))
            {
                string username = _configuration.GetValue<string>("Presets:AdminUser");
                var userService = new UserService(dbContext, _mapper, _logger);
                Assert.NotNull(userService.GetEmployeeByUsername(username));
            }
        }
    }
}
