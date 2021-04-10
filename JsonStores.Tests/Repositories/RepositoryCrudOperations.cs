using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JsonStores.Exceptions;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Repositories
{
    public class RepositoryCrudOperations : IDisposable
    {
        private readonly string _path;
        private readonly JsonStoreOptions _options;

        public RepositoryCrudOperations()
        {
            _path = Guid.NewGuid().ToString("N");
            _options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_path)};

            // create a file with an item
            var content = new[] {Constants.GetPerson()};
            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            var fullPath = Path.Combine(_options.Location, $"{_path}.json");
            File.WriteAllBytes(fullPath, jsonContent);
        }
        
        [Theory]
        [InlineData(1, true)]
        [InlineData(999, false)]
        public async Task Exists(int id, bool expected)
        {
            var store = new JsonRepository<Person, int>(_options);

            var idExists = await store.ExistsAsync(id);
            Assert.Equal(expected, idExists);
        }

        [Theory]
        [InlineData(1, "John Smith")]
        [InlineData(999, null)]
        public async Task GetById(int id, string expectedName)
        {
            var store = new JsonRepository<Person, int>(_options);

            var person = await store.GetByIdAsync(id);
            Assert.Equal(expectedName, person?.FullName);
        }

        [Fact]
        public async Task GetAll()
        {
            var store = new JsonRepository<Person, int>(_options);

            var persons = await store.GetAllAsync();

            Assert.Contains(Constants.GetPerson(), persons);
        }

        [Fact]
        public async Task Add_Success()
        {
            var store = new JsonRepository<Person, int>(_options);

            var person2 = Constants.GetPerson2();
            await store.AddAsync(person2);
            await store.SaveChangesAsync();

            // use another repository (without any cache) to load the saved item
            var newRepository = new JsonRepository<Person, int>(_options);
            var items = (await newRepository.GetAllAsync()).ToList();

            Assert.Contains(person2, items);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task Add_IdViolation()
        {
            var store = new JsonRepository<Person, int>(_options);

            await Assert.ThrowsAsync<UniquenessConstraintViolationException>(
                () => store.AddAsync(Constants.GetPerson()));
        }

        [Fact]
        public async Task SavingEmptyList()
        {
            var store = new JsonRepository<Person, int>(_options);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.SaveChangesAsync());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }
    }
}