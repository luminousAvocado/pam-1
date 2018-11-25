using PAM.Data;
using Xunit;

namespace PAM.Test.Data
{
    [Collection(nameof(DataServiceCollection))]
    public class OrganizationServiceTests
    {
        DataServiceFixture _dataServiceFixture;

        public OrganizationServiceTests(DataServiceFixture dataServiceFixture)
        {
            _dataServiceFixture = dataServiceFixture;
        }

        [Fact]
        public void GetBureausTest()
        {
            using (var dbContext = new AppDbContext(_dataServiceFixture.DbContextOptions))
            {
                var orgnizationService = new OrganizationService(dbContext);
                Assert.Equal(14, orgnizationService.GetBureaus().Count);
            }
        }
    }
}
