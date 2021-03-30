using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.Exceptions;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Options
{
    [Collection("3d0f0413-4d24-4f58-8d34-9516affe8bd5")]
    public class ThrowOnSavingChangedFile : IDisposable
    {
        private readonly string _fileName;
        private readonly JsonStoreOptions _options;

        public ThrowOnSavingChangedFile()
        {
            _fileName = Guid.NewGuid().ToString();

            // create a item and save it
            _options = new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_fileName)};
            var store = new JsonStore<Person>(_options);
            store.SaveAsync(Constants.GetPerson()).Wait();
        }

        [Fact]
        public async Task FileNotLoaded()
        {
            // change the content without load the file must be ignored
            var store = new JsonStore<Person>(_options);
            var person = Constants.GetPerson();
            person.FullName = Guid.NewGuid().ToString("N");

            await store.SaveAsync(person);

            // ensure the file was saved
            var readContent = await store.ReadAsync();
            Assert.Equal(person, readContent);
        }

        [Fact]
        public async Task FileWasLoaded()
        {
            // load the file
            var store = new JsonStore<Person>(_options);
            var person = await store.ReadAsync();

            // change it using another instance
            await Task.Run(async () =>
            {
                var store2 = new JsonStore<Person>(_options);
                person.FullName = Guid.NewGuid().ToString("N");
                await store2.SaveAsync(person);
            });

            // change it again and try to save
            var newPerson = Constants.GetPerson();
            newPerson.FullName = Guid.NewGuid().ToString("N");

            // wait to ensure the time will be different
            await Assert.ThrowsAsync<FileChangedException>(() => store.SaveAsync(newPerson));
        }

        [Fact]
        public async Task OptionIsDisabled()
        {
            // load the file
            var store = new JsonStore<Person>(_options);
            var person = await store.ReadAsync();

            // change it using another instance
            var options2 = new JsonStoreOptions(_options) {ThrowOnSavingChangedFile = false};
            var store2 = new JsonStore<Person>(options2);
            person.FullName = Guid.NewGuid().ToString("N");
            await store2.SaveAsync(person);

            // change it again and try to save
            var newPerson = Constants.GetPerson();
            newPerson.FullName = Guid.NewGuid().ToString("N");
            await store2.SaveAsync(newPerson);

            // ensure the file was saved
            var readContent = await store2.ReadAsync();
            Assert.Equal(newPerson, readContent);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            File.Delete(_fileName);
        }
    }
}