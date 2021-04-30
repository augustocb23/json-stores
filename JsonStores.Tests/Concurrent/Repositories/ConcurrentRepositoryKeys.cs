using JsonStores.Concurrent;
using JsonStores.Exceptions;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Repositories
{
    /**
     * As the validations have already been tested in class Repositories.RepositoryKey,
     * these class only asserts that the internal class RepositoryKeyValidator is called by the constructor.
     */
    public class ConcurrentRepositoryKeys
    {
        [Fact]
        public void EnsureKeyValidatorCalledOnDefaultConstructor()
        {
            var options = new JsonStoreOptions();
            Assert.Throws<InvalidJsonRepositoryKeyException>(() =>
                new ConcurrentJsonRepository<RepositoryTestModels.PersonWithoutId, int>(options));
        }
    }
}