using Xunit;

namespace PAM.Test.Data
{
    [CollectionDefinition(nameof(DataServiceCollection))]
    public class DataServiceCollection : ICollectionFixture<DataServiceFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        // See https://xunit.github.io/docs/shared-context for more details.
    }
}
