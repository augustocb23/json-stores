using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Helpers;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonRepository{T,TKey}" />
    public class ConcurrentJsonRepository<T, TKey> :
        AbstractJsonStore<ICollection<T>>, IConcurrentJsonRepository<T, TKey>
        where T : class, new() where TKey : notnull
    {
        private readonly ISemaphoreFactory _semaphoreFactory;

        /// <summary>
        ///     Creates a new instance with the given options.
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        /// <param name="semaphoreFactory">The semaphore factory.</param>
        public ConcurrentJsonRepository(JsonStoreOptions options, ISemaphoreFactory semaphoreFactory) : base(options)
        {
            _semaphoreFactory = semaphoreFactory;
            var keyProperty = RepositoryKeyValidator.GetKeyProperty<T, TKey>();
            GetKeyValue = keyProperty.Compile();
        }

        /// <summary>
        ///     Creates a new instance with the given options and key.
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        /// <param name="keyProperty">A <see cref="Func{TResult}" /> to get the object's key.</param>
        /// <param name="semaphoreFactory">The semaphore factory.</param>
        public ConcurrentJsonRepository(
            JsonStoreOptions options,
            Expression<Func<T, TKey>> keyProperty,
            ISemaphoreFactory semaphoreFactory
        ) : base(options)
        {
            _semaphoreFactory = semaphoreFactory;
            GetKeyValue = keyProperty.Compile();
        }

        /// <summary>
        ///     Method used to obtain an object's key value.
        /// </summary>
        private Func<T, TKey> GetKeyValue { get; }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(T item)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public async Task AddAndSaveAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAndSaveAsync(T item)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAndSaveAsync(TKey id)
        {
            throw new NotImplementedException();
        }
    }
}