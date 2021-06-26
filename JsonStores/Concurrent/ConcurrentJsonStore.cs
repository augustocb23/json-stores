using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonStore{T}" />
    public class ConcurrentJsonStore<T> : AbstractJsonStore<T>, IConcurrentJsonStore<T>
        where T : class, new()
    {
        private readonly ISemaphoreFactory _semaphoreFactory;

        /// <summary>
        ///     Creates a new instance of <see cref="ConcurrentJsonStore{T}" /> with the given options.
        /// </summary>
        /// <param name="options">The options for this store.</param>
        /// <param name="semaphoreFactory">The semaphore factory.</param>
        public ConcurrentJsonStore(JsonStoreOptions options, ISemaphoreFactory semaphoreFactory) : base(options)
        {
            _semaphoreFactory = semaphoreFactory;
        }

        public async Task<T> ReadAsync()
        {
            if (!FileExists) return null;
            return await ReadItemAsync();
        }

        public async Task<T> ReadOrCreateAsync()
        {
            if (!FileExists) return new T();
            return await ReadItemAsync();
        }

        public async Task SaveAsync(T item)
        {
            var semaphore = _semaphoreFactory.GetSemaphore<T>();

            try
            {
                semaphore.WaitOne();
                await SaveToFileAsync(item);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<T> ReadItemAsync()
        {
            var semaphore = _semaphoreFactory.GetSemaphore<T>();

            try
            {
                semaphore.WaitOne();
                return await ReadFileAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}