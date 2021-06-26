using System;
using System.Threading;
using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Semaphores
{
    public class NamedSemaphore
    {
        private readonly string _semaphoreName;

        public NamedSemaphore()
        {
            _semaphoreName = Guid.NewGuid().ToString();
        }

        [Fact]
        public void SameFactory()
        {
            ISemaphoreFactory factory = new NamedSemaphoreFactory(_semaphoreName);

            var expected = factory.GetSemaphore<Person>();
            var actual = factory.GetSemaphore<Person>();

            Assert.NotNull(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DiffFactories()
        {
            using var expectedSemaphore = new Semaphore(1, 1, _semaphoreName);
            ISemaphoreFactory factory = new NamedSemaphoreFactory(_semaphoreName);

            var createdSemaphore = factory.GetSemaphore<Person>();
            Assert.NotNull(createdSemaphore);

            // wait two seconds then releases the semaphore
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                createdSemaphore.Release();
            });

            // lock the semaphore and wait until four seconds to release
            expectedSemaphore.WaitOne();
            var released = expectedSemaphore.WaitOne(4000);
            Assert.True(released);
        }
    }
}