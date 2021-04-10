using System;
using System.IO;
using System.Threading.Tasks;
using JsonStores.Exceptions;
using JsonStores.NamingStrategies;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Repositories
{
    public class RepositoryKey : IDisposable
    {
        private readonly string _path;

        public RepositoryKey()
        {
            _path = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public void CreateRepositoryWithoutKey_Id()
        {
            var options = new JsonStoreOptions();
            Assert.Throws<InvalidJsonRepositoryKeyException>(() =>
                new JsonRepository<RepositoryTestModels.PersonWithoutId, int>(options));
        }

        [Fact]
        public void CreateRepositoryWithWrongKeyType_Id()
        {
            var options = new JsonStoreOptions();
            Assert.Throws<InvalidJsonRepositoryKeyException>(() => new JsonRepository<Person, string>(options));
        }

        [Fact]
        public void CreateRepositoryWithWrongKeyType_Attribute()
        {
            var options = new JsonStoreOptions();
            Assert.Throws<InvalidJsonRepositoryKeyException>(() =>
                new JsonRepository<RepositoryTestModels.PersonIdWithAttribute, string>(options));
        }

        [Fact]
        public async Task CreateRepositoryWithKey_Attribute()
        {
            IJsonRepository<RepositoryTestModels.PersonIdWithAttribute, int> GetRepository() =>
                new JsonRepository<RepositoryTestModels.PersonIdWithAttribute, int>(new JsonStoreOptions
                    {NamingStrategy = new StaticNamingStrategy(_path)});

            var repository = GetRepository();

            // add an item
            const int key = 99;
            var item = new RepositoryTestModels.PersonIdWithAttribute {Id = 1, Number = key, Name = "Number Nine-Nine"};
            await repository.AddAsync(item);
            await repository.SaveChangesAsync();

            // use another repository (without any cache) to load the saved item
            var newRepository = GetRepository();
            var newItem = await newRepository.GetByIdAsync(key);

            Assert.Equal(item, newItem);
        }


        [Fact]
        public async Task CreateRepositoryWithLambda()
        {
            IJsonRepository<RepositoryTestModels.PersonWithoutId, int> GetRepository() =>
                new JsonRepository<RepositoryTestModels.PersonWithoutId, int>(
                    new JsonStoreOptions {NamingStrategy = new StaticNamingStrategy(_path)},
                    person => person.Number);

            // add an item
            var repository = GetRepository();
            const int key = 99;
            var item = new RepositoryTestModels.PersonWithoutId {Number = key, Name = "Number Nine-Nine"};
            await repository.AddAsync(item);
            await repository.SaveChangesAsync();

            // use another repository (without any cache) to load the saved item
            var newRepository = GetRepository();
            var newItem = await newRepository.GetByIdAsync(key);

            Assert.Equal(item, newItem);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_path != null) File.Delete(_path);
        }
    }
}