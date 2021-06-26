using JsonStores.Concurrent;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Tests.Models;

namespace JsonStores.Tests.Repositories
{
    public class ConcurrentJsonRepositoryTest : AbstractJsonRepositoryTest<ConcurrentJsonRepository<Person, int>>
    {
        protected override ConcurrentJsonRepository<Person, int> GetStore(JsonStoreOptions options)
        {
            return new(options, new LocalSemaphoreFactory());
        }
    }
}