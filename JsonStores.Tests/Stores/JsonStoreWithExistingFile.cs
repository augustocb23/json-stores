using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Stores
{
    public class JsonStoreWithExistingFile : IDisposable
    {
        private readonly string _fileName;
        private readonly JsonStore<Person> _store;
        private readonly Person _content;

        public JsonStoreWithExistingFile()
        {
            _fileName = Guid.NewGuid().ToString();
            _content = new Person
            {
                Id = 1,
                FullName = "John",
                BirthDate = DateTime.UnixEpoch
            };

            // create a item and save it using a temporally store
            var options = new JsonStoreOptions
            {
                NamingStrategy = new StaticNamingStrategy(_fileName)
            };
            var store = new JsonStore<Person>(options);
            _content = new Person
            {
                Id = 1,
                FullName = "John Smith",
                BirthDate = DateTime.UnixEpoch
            };
            store.SaveAsync(_content).Wait();

            _store = new JsonStore<Person>(options);
        }

        [Fact]
        public async Task Read()
        {
            var person = await _store.ReadAsync();

            Assert.Equal(person, _content);
        }

        [Fact]
        public async Task ReadOrCreate()
        {
            var person = await _store.ReadOrCreateAsync();

            Assert.Equal(person, _content);
        }

        [Fact]
        public async Task ChangeAnItem()
        {
            _content.FullName = "Thomas Smith";
            await _store.SaveAsync(_content);

            var person = await _store.ReadAsync();
            Assert.Equal(person, _content);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_fileName);
        }
    }
}