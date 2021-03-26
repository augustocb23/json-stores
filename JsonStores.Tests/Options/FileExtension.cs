using System;
using System.IO;
using System.Threading.Tasks;
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
            var options = new JsonStoreOptions();
            _path = $@"{options.Location}\{nameof(Person)}.json";
            var store = new JsonStore<Person>(options);

            await store.SaveAsync(_person);

            Assert.True(File.Exists(_path));
        } 
        
        [Fact]
        public async Task Generated()
        {
            var extension = Guid.NewGuid().ToString("N"); 
            var options = new JsonStoreOptions{FileExtension = extension};
            _path = $@"{options.Location}\{nameof(Person)}.{extension}";
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