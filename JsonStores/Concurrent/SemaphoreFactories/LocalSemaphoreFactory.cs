using System;
using System.Threading;

namespace JsonStores.Concurrent.SemaphoreFactories
{
    /// <summary>
    ///     A factory that always returns the same application-level semaphore.
    /// </summary>
    public class LocalSemaphoreFactory : ISemaphoreFactory
    {
        private Semaphore _semaphore;

        public Semaphore GetSemaphore<T>()
        {
            return _semaphore ??= new Semaphore(1, 1);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _semaphore?.Dispose();
        }
    }
}