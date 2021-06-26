using JsonStores.Tests.Models;

namespace JsonStores.Tests.Repositories
{
    public class JsonRepositoryTest : AbstractJsonRepositoryTest<JsonRepository<Person, int>>
    {
        protected override JsonRepository<Person, int> GetStore(JsonStoreOptions options)
        {
            return new(options);
        }
    }
}