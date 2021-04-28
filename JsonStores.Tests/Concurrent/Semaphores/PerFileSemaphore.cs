using System;
using System.Threading;
using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Tests.Models;
using Xunit;

namespace JsonStores.Tests.Concurrent.Semaphores
{
    public class PerFileSemaphore : IDisposable
    {
        private readonly ISemaphoreFactory _factory;
        private readonly JsonStoreOptions _options;

        public PerFileSemaphore()
        {
            // create a option to simulate a file
            _options = new JsonStoreOptions();
            _factory = new PerFileSemaphoreFactory(_options);
        }

        [Fact]
        public void SameFactory()
        {
            var semaphore1 = _factory.GetSemaphore<Person>();
            var semaphore2 = _factory.GetSemaphore<Person>();

            // wait for a small task
            var released = SimulateFileOperation(
                semaphore1, 2000, semaphore2, 4000);

            // must be the same instance and receive a signal
            Assert.Equal(semaphore1, semaphore2);
            Assert.True(released);
        }


        [Fact]
        public void DiffFactory()
        {
            var semaphore1 = _factory.GetSemaphore<Person>();

            var factory2 = new PerFileSemaphoreFactory(_options);
            var semaphore2 = factory2.GetSemaphore<Person>();

            // wait for a small task
            var released = SimulateFileOperation(
                semaphore1, 2000, semaphore2, 4000);

            // must be different instances, but still receives a signal
            Assert.NotEqual(semaphore1, semaphore2);
            Assert.True(released);
        }

        [Fact]
        public void DiffFiles()
        {
            var personSemaphore = _factory.GetSemaphore<Person>();
            var intSemaphore = _factory.GetSemaphore<int>();

            var released = SimulateFileOperation(
                personSemaphore, 5000, intSemaphore, 2000);

            // must be different instances and never lock
            Assert.NotEqual(personSemaphore, intSemaphore);
            Assert.True(released);
        }

        /// <summary>
        ///     Use a task to simulate a file operation and checks if second semaphore will receive a signal before <paramref name="waitDelay"/>.
        /// </summary>
        /// <param name="taskSemaphore">First semaphore.</param>
        /// <param name="taskDelay">Task duration in milliseconds.</param>
        /// <param name="waitSemaphore">Second semaphore.</param>
        /// <param name="waitDelay">Waiting time for a signal.</param>
        /// <returns><c>True</c> if the semaphore received a signal. <c>False</c> otherwise.</returns>
        private static bool SimulateFileOperation(
            Semaphore taskSemaphore, int taskDelay,
            Semaphore waitSemaphore, int waitDelay)
        {
            // simulate opening of the file using a task
            var _ = Task.Run(async () =>
            {
                await Task.Delay(taskDelay);
                taskSemaphore.Release();
            });

            // lock the semaphore and wait to release
            taskSemaphore.WaitOne();
            var released = waitSemaphore.WaitOne(waitDelay);
            waitSemaphore.Release();

            return released;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _factory.Dispose();
        }
    }
}