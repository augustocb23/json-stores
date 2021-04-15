using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Helpers;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Stores
{
    public class JsonStoreWithNoFile : IDisposable
    {
        private readonly string _fileName;
        private readonly JsonStore<Person> _store;

        public JsonStoreWithNoFile()
        {
            _fileName = Guid.NewGuid().ToString();
            var options = new JsonStoreOptions
            {
                NamingStrategy = new StaticNamingStrategy(_fileName)
            };
            _store = new JsonStore<Person>(options);
        }

        [Fact]
        public async Task ReadFile_null()
        {
            var person = await _store.ReadAsync();

            Assert.Null(person);
        }

        [Fact]
        public async Task ReadFile_new()
        {
            var person = await _store.ReadOrCreateAsync();

            Assert.Equal(person, new Person());
        }

        [Fact]
        public async Task SaveAndRead()
        {
            var person = new Person
            {
                FullName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.UnixEpoch
            };
            await _store.SaveAsync(person);

            var readPerson = await _store.ReadAsync();

            Assert.Equal(person, readPerson);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(FilePathEvaluator.GetFilePath(_fileName));
        }
    }
}