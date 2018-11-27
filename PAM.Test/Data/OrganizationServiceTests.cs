using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAM.Data;
using Xunit;

namespace PAM.Test.Data
{
    [Collection(nameof(DataServiceCollection))]
    public class OrganizationServiceTests
    {
        readonly DbContextOptions<AppDbContext> _dbContextOptions;
        readonly ILogger<OrganizationService> _logger;

        public OrganizationServiceTests(DataServiceFixture dataServiceFixture)
        {
            _dbContextOptions = dataServiceFixture.DbContextOptions;
            _logger = dataServiceFixture.LoggerFactory.CreateLogger<OrganizationService>();
        }

        [Fact]
        public void GetBureausTest()
        {
            using (var dbContext = new AppDbContext(_dbContextOptions))
            {
                var orgnizationService = new OrganizationService(dbContext, _logger);
                Assert.Equal(14, orgnizationService.GetBureaus().Count);
            }
        }
    }
}
