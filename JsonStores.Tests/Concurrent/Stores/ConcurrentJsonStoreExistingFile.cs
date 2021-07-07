using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.Concurrent;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Stores
{
    public class ConcurrentJsonStoreExistingFile : IDisposable
    {
        private readonly string _fileName;
        private readonly JsonStoreOptions _options;
        private readonly ISemaphoreFactory _semaphoreFactory;

        public ConcurrentJsonStoreExistingFile()
        {
            // creates a file
            _fileName = Guid.NewGuid().ToString();
            _semaphoreFactory = new LocalSemaphoreFactory();

            // create a file
            _options = new JsonStoreOptions
            {
                NamingStrategy = new StaticNamingStrategy(_fileName)
            };
            var filePath = Path.Combine(_options.Location, $"{_fileName}.json");
            JsonFileCreator.CreateStore(filePath);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_fileName);
        }

        [Fact]
        public async Task ReadItem()
        {
            var store = new ConcurrentJsonStore<Person>(_options, _semaphoreFactory);

            var person = await store.ReadAsync();
            Assert.Equal(Constants.GetPerson(), person);
        }

        [Fact]
        public void ReadAndSave()
        {
            var store = new ConcurrentJsonStore<Person>(_options, _semaphoreFactory);
            var person2 = Constants.GetPerson2();

            // use a task to change the item and other to read it
            Task.WaitAll(
                Task.Run(async () => await store.SaveAsync(person2)),
                Task.Run(async () => await store.ReadAsync())
            );
        }

        [Fact]
        public async Task ChangeItem()
        {
            var store1 = new ConcurrentJsonStore<Person>(_options, _semaphoreFactory);
            var newPerson = Constants.GetPerson2();
            await store1.SaveAsync(newPerson);

            // use another store to read
            var store2 = new ConcurrentJsonStore<Person>(_options, _semaphoreFactory);
            var person = await store2.ReadAsync();
            Assert.Equal(Constants.GetPerson2(), person);
        }
    }
}