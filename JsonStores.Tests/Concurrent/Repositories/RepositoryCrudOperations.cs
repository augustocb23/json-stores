using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonStores.Concurrent;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Exceptions;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Repositories
{
    public class RepositoryCrudOperations : IDisposable
    {
        private readonly JsonStoreOptions _options;
        private readonly string _path;
        private readonly ISemaphoreFactory _semaphoreFactory;

        public RepositoryCrudOperations()
        {
            _path = Guid.NewGuid().ToString("N");
            _options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_path)};
            _semaphoreFactory = new LocalSemaphoreFactory();

            // create a file with an item
            var filePath = Path.Combine(_options.Location, $"{_path}.json");
            JsonFileCreator.CreateSingleItemRepository(filePath);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(999, false)]
        public async Task Exists(int id, bool expected)
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            var idExists = await store.ExistsAsync(id);
            Assert.Equal(expected, idExists);
        }

        [Theory]
        [InlineData(1, "John Smith")]
        [InlineData(999, null)]
        public async Task GetById(int id, string expectedName)
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            var person = await store.GetByIdAsync(id);
            Assert.Equal(expectedName, person?.FullName);
        }

        [Fact]
        public async Task GetAll()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            var persons = await store.GetAllAsync();

            Assert.Contains(Constants.GetPerson(), persons);
        }

        [Fact]
        public async Task Add_Success()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            var person2 = Constants.GetPerson2();
            await store.AddAsync(person2);
            await store.SaveChangesAsync();

            // use another repository (without any cache) to load the saved item
            var newRepository = new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
            var items = (await newRepository.GetAllAsync()).ToList();

            Assert.Contains(person2, items);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task Add_IdViolation()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            await Assert.ThrowsAsync<UniquenessConstraintViolationException>(
                () => store.AddAsync(Constants.GetPerson()));
        }

        [Fact]
        public async Task Update_NotExisting()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
            var notExistingItem = new Person {Id = 999, FullName = "Nobody"};

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await store.UpdateAsync(notExistingItem));
        }

        [Fact]
        public async Task Update_Success()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
            var person = Constants.GetPerson();
            person.FullName = Guid.NewGuid().ToString();

            await store.UpdateAsync(person);
            await store.SaveChangesAsync();

            // reloads the item
            IConcurrentJsonRepository<Person, int> newStore =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
            var newPerson = await newStore.GetByIdAsync(person.Id);

            Assert.Equal(person, newPerson);
        }

        [Fact]
        public async Task Remove_NotExisting()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await store.RemoveAsync(999));
        }

        [Fact]
        public async Task Remove_Success()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            await store.RemoveAsync(1);
            await store.SaveChangesAsync();

            IConcurrentJsonRepository<Person, int> newStore =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);
            var items = await newStore.GetAllAsync();
            Assert.Empty(items);
        }

        [Fact]
        public async Task SavingEmptyList()
        {
            IConcurrentJsonRepository<Person, int> store =
                new ConcurrentJsonRepository<Person, int>(_options, _semaphoreFactory);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.SaveChangesAsync());
        }
    }
}