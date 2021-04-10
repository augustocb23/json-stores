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
        private readonly IJsonStore<Person> _store;
        private readonly Person _content;

        public JsonStoreWithExistingFile()
        {
            _fileName = Guid.NewGuid().ToString();
            _content = Constants.GetPerson();

            // create a file
            var options = new JsonStoreOptions
            {
                NamingStrategy = new StaticNamingStrategy(_fileName)
            };
            var filePath = Path.Combine(options.Location, $"{_fileName}.json");
            JsonFileCreator.CreateStore(filePath);

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
            var previousContent = await _store.ReadAsync();

            var newContent = Constants.GetPerson();
            newContent.FullName = "Thomas Smith";
            await _store.SaveAsync(newContent);

            var person = await _store.ReadAsync();
            Assert.Equal(person, newContent);
            Assert.NotEqual(person, previousContent);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_fileName);
        }
    }
}