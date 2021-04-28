using System;
using System.Reflection;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Semaphores
{
    public class LocalSemaphore : IDisposable
    {
        private readonly ISemaphoreFactory _factory;

        public LocalSemaphore()
        {
            _factory = new LocalSemaphoreFactory();
        }

        [Fact]
        public void SameType()
        {
            var expected = _factory.GetSemaphore<Person>();
            var actual = _factory.GetSemaphore<Person>();

            Assert.NotNull(expected);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(Person))]
        public void DiffType(Type type)
        {
            // gets the generic method using reflection
            var method = typeof(ISemaphoreFactory).GetMethod(nameof(ISemaphoreFactory.GetSemaphore))?
                .MakeGenericMethod(type);
            Assert.NotNull(method);

            var expected = _factory.GetSemaphore<int>();
            var actual = method.Invoke(_factory, null);

            Assert.NotNull(expected);
            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _factory?.Dispose();
        }
    }
}