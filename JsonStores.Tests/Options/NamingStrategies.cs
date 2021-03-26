using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Options
{
    public class NamingStrategies : IDisposable
    {
        private readonly Person _person;
        private string _path;

        public NamingStrategies()
        {
            _person = Constants.GetPerson();
        }

        [Fact]
        public async Task StaticName()
        {
            var options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy("static-file-name")};
            _path = $@"{options.Location}\static-file-name.{options.FileExtension}";
            var store = new JsonStore<Person>(options);

            await store.SaveAsync(_person);

            Assert.True(File.Exists(_path));
        }

        [Fact]
        public async Task ClassName()
        {
            var options = new JsonStoreOptions();
            _path = $@"{options.Location}\{nameof(Person)}.{options.FileExtension}";
            var store = new JsonStore<Person>(options);

            await store.SaveAsync(_person);

            Assert.True(File.Exists(_path));
        }

        [Fact]
        public async Task ClassName_WithGenerics()
        {
            var options = new JsonStoreOptions();
            _path = $@"{options.Location}\{nameof(Person)}.{options.FileExtension}";
            var store = new JsonRepository<Person, int>(options);

            await store.AddAsync(_person);
            await store.SaveChangesAsync();

            Assert.True(File.Exists(_path));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }
    }
}