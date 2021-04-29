using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonRepository{T,TKey}"/>
    public class ConcurrentJsonRepository<T, TKey> : IConcurrentJsonRepository<T, TKey>
        where T : class, new() where TKey : notnull
    {
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddAsync(T item)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsync(T item)
        {
            throw new System.NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> ExistsAsync(TKey id)
        {
            throw new System.NotImplementedException();
        }

        public async Task RemoveAsync(TKey id)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddAndSaveAsync(T obj)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAndSaveAsync(T item)
        {
            throw new System.NotImplementedException();
        }

        public async Task RemoveAndSaveAsync(TKey id)
        {
            throw new System.NotImplementedException();
        }
    }
}