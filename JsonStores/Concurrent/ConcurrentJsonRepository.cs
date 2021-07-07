using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JsonStores.Concurrent.SemaphoreFactories;
using JsonStores.Exceptions;
using JsonStores.Helpers;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonRepository{T,TKey}" />
    public class ConcurrentJsonRepository<T, TKey> :
        AbstractJsonStore<ICollection<T>>, IConcurrentJsonRepository<T, TKey>
        where T : class, new() where TKey : notnull
    {
        private readonly Semaphore _semaphore;

        /// <summary>
        ///     Creates a new instance with the given options.
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        /// <param name="semaphoreFactory">The semaphore factory.</param>
        public ConcurrentJsonRepository(JsonStoreOptions options, ISemaphoreFactory semaphoreFactory) : base(options)
        {
            var keyProperty = RepositoryKeyValidator.GetKeyProperty<T, TKey>();
            GetKeyValue = keyProperty.Compile();

            _semaphore = semaphoreFactory.GetSemaphore<T>();
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
            GetKeyValue = keyProperty.Compile();

            _semaphore = semaphoreFactory.GetSemaphore<T>();
        }

        /// <summary>
        ///     A collections containing the loaded data.
        /// </summary>
        private ICollection<T> Items { get; set; }

        /// <summary>
        ///     Method used to obtain an object's key value.
        /// </summary>
        private Func<T, TKey> GetKeyValue { get; }

        /// <summary>
        ///     Load (or initialize) current content.
        /// </summary>
        /// <returns>Current content of the collection.</returns>
        private async Task LoadAsync()
        {
            if (FileExists) Items = await ReadFileAsync();
            else Items = new List<T>();
        }

        #region IRepository implementation (wrap operations with semaphore)

        /// <inheritdoc />
        public Task<IEnumerable<T>> GetAllAsync()
        {
            _semaphore.WaitOne();
            try
            {
                return GetAllOperation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task<T> GetByIdAsync(TKey id)
        {
            _semaphore.WaitOne();
            try
            {
                return GetByIdOperation(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task AddAsync(T item)
        {
            _semaphore.WaitOne();
            try
            {
                return AddOperation(item);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task UpdateAsync(T item)
        {
            _semaphore.WaitOne();
            try
            {
                return UpdateOperation(item);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(TKey id)
        {
            _semaphore.WaitOne();
            try
            {
                return ExistsOperation(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task RemoveAsync(TKey id)
        {
            _semaphore.WaitOne();
            try
            {
                return RemoveOperation(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task SaveChangesAsync()
        {
            if (Items == null)
                throw new InvalidOperationException("Content was never loaded.");

            _semaphore.WaitOne();
            try
            {
                return SaveChangesOperation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #endregion

        #region IConcurrentRepository implementation

        public async Task AddAndSaveAsync(T item)
        {
            _semaphore.WaitOne();
            try
            {
                await AddOperation(item);
                await SaveChangesOperation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdateAndSaveAsync(T item)
        {
            _semaphore.WaitOne();
            try
            {
                await UpdateOperation(item);
                await SaveChangesOperation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveAndSaveAsync(TKey id)
        {
            _semaphore.WaitOne();
            try
            {
                await RemoveOperation(id);
                await SaveChangesOperation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #endregion

        #region Repository operations

        private async Task<IEnumerable<T>> GetAllOperation()
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items;
        }

        private async Task<T> GetByIdOperation(TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
        }

        private async Task AddOperation(T item)
        {
            if (await ExistsOperation(GetKeyValue(item)))
                throw new UniquenessConstraintViolationException(item, GetKeyValue(item));

            Items.Add(item);
        }

        private async Task UpdateOperation(T item)
        {
            if (Items == null || FileChanged) await LoadAsync();

            await RemoveOperation(GetKeyValue(item));
            await AddOperation(item);
        }

        private async Task<bool> ExistsOperation(TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            return Items.Any(item => id.Equals(GetKeyValue(item)));
        }

        private async Task RemoveOperation(TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            var obj = Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
            if (obj == null) throw new ItemNotFoundException(id);

            Items.Remove(obj);
        }

        private async Task SaveChangesOperation()
        {
            // don't need to check for null if is using IConcurrentRepository methods

            await SaveToFileAsync(Items);
        }

        #endregion
    }
}