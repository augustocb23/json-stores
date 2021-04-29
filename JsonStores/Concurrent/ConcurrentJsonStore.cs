using System;
using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonStore{T}"/>
    public class ConcurrentJsonStore<T> : AbstractJsonStore<T>, IConcurrentJsonStore<T>, IDisposable
        where T : class, new()
    {
        private readonly ISemaphoreFactory _semaphoreFactory;

        /// <summary>
        ///     Creates a new instance of <see cref="ConcurrentJsonStore{T}"/> with the given options. 
        /// </summary>
        /// <param name="options">The options for this store.</param>
        /// <param name="semaphoreFactory">The semaphore factory.</param>
        public ConcurrentJsonStore(JsonStoreOptions options, ISemaphoreFactory semaphoreFactory) : base(options)
        {
            _semaphoreFactory = semaphoreFactory;
        }

        public async Task<T> ReadAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> ReadOrCreateAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task SaveAsync(T obj)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _semaphoreFactory.Dispose();
        }
    }
}