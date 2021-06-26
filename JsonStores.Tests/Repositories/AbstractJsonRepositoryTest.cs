using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonStores.Exceptions;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Repositories
{
    public abstract class AbstractJsonRepositoryTest<T> : IDisposable where T : IJsonRepository<Person, int>
    {
        private readonly JsonStoreOptions _options;
        private readonly string _path;

        protected AbstractJsonRepositoryTest()
        {
            _path = Guid.NewGuid().ToString("N");
            _options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_path)};

            // create a file with an item
            var filePath = Path.Combine(_options.Location, $"{_path}.json");
            JsonFileCreator.CreateSingleItemRepository(filePath);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }

        protected abstract T GetStore(JsonStoreOptions options);

        [Theory]
        [InlineData(1, true)]
        [InlineData(999, false)]
        public async Task Exists(int id, bool expected)
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            var idExists = await store.ExistsAsync(id);
            Assert.Equal(expected, idExists);
        }

        [Theory]
        [InlineData(1, "John Smith")]
        [InlineData(999, null)]
        public async Task GetById(int id, string expectedName)
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            var person = await store.GetByIdAsync(id);
            Assert.Equal(expectedName, person?.FullName);
        }

        [Fact]
        public async Task GetAll()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            var persons = await store.GetAllAsync();

            Assert.Contains(Constants.GetPerson(), persons);
        }

        [Fact]
        public async Task Add_Success()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            var person2 = Constants.GetPerson2();
            await store.AddAsync(person2);
            await store.SaveChangesAsync();

            // use another repository (without any cache) to load the saved item
            var newRepository = GetStore(_options);
            var items = (await newRepository.GetAllAsync()).ToList();

            Assert.Contains(person2, items);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task Add_IdViolation()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            await Assert.ThrowsAsync<UniquenessConstraintViolationException>(
                () => store.AddAsync(Constants.GetPerson()));
        }

        [Fact]
        public async Task Update_NotExisting()
        {
            IJsonRepository<Person, int> store = GetStore(_options);
            var notExistingItem = new Person {Id = 999, FullName = "Nobody"};

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await store.UpdateAsync(notExistingItem));
        }

        [Fact]
        public async Task Update_Success()
        {
            IJsonRepository<Person, int> store = GetStore(_options);
            var person = Constants.GetPerson();
            person.FullName = Guid.NewGuid().ToString();

            await store.UpdateAsync(person);
            await store.SaveChangesAsync();

            // reloads the item
            IJsonRepository<Person, int> newStore = GetStore(_options);
            var newPerson = await newStore.GetByIdAsync(person.Id);

            Assert.Equal(person, newPerson);
        }

        [Fact]
        public async Task Remove_NotExisting()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await store.RemoveAsync(999));
        }

        [Fact]
        public async Task Remove_Success()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            await store.RemoveAsync(1);
            await store.SaveChangesAsync();

            IJsonRepository<Person, int> newStore = GetStore(_options);
            var items = await newStore.GetAllAsync();
            Assert.Empty(items);
        }

        [Fact]
        public async Task SavingEmptyList()
        {
            IJsonRepository<Person, int> store = GetStore(_options);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.SaveChangesAsync());
        }
    }
}