using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        readonly ILogger<UserService> _logger;

        public UserServiceTests(DataServiceFixture dataServiceFixture)
        {
            _configuration = dataServiceFixture.Configuration;
            _dbContextOptions = dataServiceFixture.DbContextOptions;
            _logger = dataServiceFixture.LoggerFactory.CreateLogger<UserService>();
        }

        [Fact]
        public void GetAdminTest()
        {
            using (var dbContext = new AppDbContext(_dbContextOptions, _configuration))
            {
                string username = _configuration.GetValue<string>("Presets:AdminUser");
                var userService = new UserService(dbContext, _logger);
                Assert.NotNull(userService.GetEmployee(username));
            }
        }
    }
}
