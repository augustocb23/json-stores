using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JsonStores.Concurrent;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Repositories
{
    public class ConcurrentRepositoryCrudOperations : IDisposable
    {
        private readonly JsonStoreOptions _options;
        private readonly string _path;
        private readonly ISemaphoreFactory _semaphoreFactory;
        private readonly CancellationTokenSource _tokenSource;

        public ConcurrentRepositoryCrudOperations()
        {
            _path = Guid.NewGuid().ToString("N");
            _options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_path)};
            _semaphoreFactory = new LocalSemaphoreFactory();
            _tokenSource = new CancellationTokenSource();

            // create a file with an item
            var filePath = Path.Combine(_options.Location, $"{_path}.json");
            JsonFileCreator.CreateMultiItemsRepository(filePath);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }

        [Fact]
        public void AddAndSave()
        {
            var store1 = GetConcurrentRepository();
            var store2 = GetConcurrentRepository();
            var person3 = new Person();

            // use a task to change the item and other to read it
            RunWithTimeout(Task.Run(async () =>
            {
                await Task.WhenAll(
                    Task.Run(async () => await store1.AddAndSaveAsync(person3)),
                    Task.Run(async () => await store2.GetAllAsync()));
            }), 5000);
        }

        [Fact]
        public void UpdateAndSave()
        {
            var store1 = GetConcurrentRepository();
            var store2 = GetConcurrentRepository();

            // use a task to change the item and other to read it
            RunWithTimeout(Task.Run(async () =>
            {
                var person = await store1.GetByIdAsync(1);
                person.FullName = Guid.NewGuid().ToString("N");

                await Task.WhenAll(
                    Task.Run(async () => await store1.UpdateAndSaveAsync(person)),
                    Task.Run(async () => await store2.GetAllAsync()));

                var store3 = GetConcurrentRepository();
                var updatedPerson = await store3.GetByIdAsync(1);

                Assert.Equal(person, updatedPerson);
            }), 5000);
        }


        [Fact]
        public void RemoveAndSave()
        {
            var store1 = GetConcurrentRepository();
            var store2 = GetConcurrentRepository();

            // use a task to change the item and other to read it
            RunWithTimeout(Task.Run(async () =>
            {
                await Task.WhenAll(
                    Task.Run(async () => await store1.RemoveAndSaveAsync(1)),
                    Task.Run(async () => await store2.GetAllAsync()));

                var store3 = GetConcurrentRepository();
                var removedPerson = await store3.GetByIdAsync(1);

                Assert.Null(removedPerson);
            }), 5000);
        }

        private void RunWithTimeout(Task task, int timeout)
        {
            Task.WaitAny(Task.Run(async () =>
            {
                await Task.Delay(timeout, _tokenSource.Token);
                _tokenSource.Cancel();
            }), task);
        }

        private IConcurrentJsonRepository<Person, int> GetConcurrentRepository()
        {
            return new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
        }
    }
}