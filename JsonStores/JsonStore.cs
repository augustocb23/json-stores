using System.Threading.Tasks;

namespace JsonStores
{
    /// <inheritdoc cref="IJsonStore{T}"/>
    public class JsonStore<T> : AbstractJsonStore<T>, IJsonStore<T> where T : class, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="JsonStore{T}"/> with the given options. 
        /// </summary>
        /// <param name="options">The options for this store.</param>
        public JsonStore(JsonStoreOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        public async Task<T> ReadAsync()
        {
            if (!FileExists) return null;
            return await ReadFileAsync();
        }

        /// <inheritdoc />
        public virtual async Task<T> ReadOrCreateAsync()
        {
            if (FileExists) return await ReadFileAsync();
            return new T();
        }

        /// <inheritdoc />
        public async Task SaveAsync(T obj)
        {
            await SaveToFileAsync(obj);
        }
    }
}