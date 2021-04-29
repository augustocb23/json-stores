using System;
using System.Threading.Tasks;
using JsonStores.Concurrent;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Stores
{
    public class ConcurrentJsonStoresNoFile : IDisposable
    {
        private readonly ConcurrentJsonStore<Person> _store;

        public ConcurrentJsonStoresNoFile()
        {
            var options = new JsonStoreOptions
                {NamingStrategy = new StaticNamingStrategy(Guid.NewGuid().ToString())};
            _store = new ConcurrentJsonStore<Person>(options, new LocalSemaphoreFactory());
        }

        [Fact]
        public async Task GetNullValue()
        {
            var person = await _store.ReadAsync();

            Assert.Null(person);
        }

        [Fact]
        public async Task GetNewValue()
        {
            var expected = new Person();
            var person = await _store.ReadOrCreateAsync();

            Assert.Equal(expected, person);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _store?.Dispose();
        }
    }
}