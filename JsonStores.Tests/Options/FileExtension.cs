using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Helpers;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Options
{
    public class FileExtension : IDisposable
    {
        private readonly Person _person;
        private string _path;

        public FileExtension()
        {
            _person = Constants.GetPerson();
        }

        [Fact]
        public async Task JsonExtension()
        {
            var fileName = Guid.NewGuid().ToString();
            var options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(fileName)};
            _path = FilePathEvaluator.GetFilePath(fileName, "json");
            var store = new JsonStore<Person>(options);

            await store.SaveAsync(_person);

            Assert.True(File.Exists(_path));
        }

        [Fact]
        public async Task Generated()
        {
            var extension = Guid.NewGuid().ToString("N");
            var options = new JsonStoreOptions {FileExtension = extension};
            _path = FilePathEvaluator.GetFilePath(nameof(Person), extension);
            var store = new JsonStore<Person>(options);

            await store.SaveAsync(_person);

            Assert.True(File.Exists(_path));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_path);
        }
    }
}