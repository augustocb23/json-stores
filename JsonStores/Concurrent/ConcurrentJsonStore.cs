using System.Threading.Tasks;

namespace JsonStores.Concurrent
{
    /// <inheritdoc cref="IConcurrentJsonStore{T}"/>
    public class ConcurrentJsonStore<T> : IConcurrentJsonStore<T> where T : class, new()
    {
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
    }
}