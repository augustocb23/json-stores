using System.Threading.Tasks;

namespace JsonStores
{
    /// <summary>
    ///     An implementation for <see cref="AbstractJsonStore{T}" /> to store a single item.
    /// </summary>
    /// <typeparam name="T">The type for the item to be persisted.</typeparam>
    public class JsonStore<T> : AbstractJsonStore<T>, IJsonStore<T> where T : class, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="JsonStore{T}"/> with the given options. 
        /// </summary>
        /// <param name="options">The options for this store.</param>
        public JsonStore(JsonStoreOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Reads the configuration file, if exists, and return its value.
        /// </summary>
        /// <returns>The current value, if the file exists. <c>Null</c> otherwise.</returns>
        public async Task<T> ReadAsync()
        {
            if (!FileExists) return null;
            return await ReadFileAsync();
        }

        /// <summary>
        ///     Reads the configuration file and return its value. If the file don't exists, returns an empty object.
        /// </summary>
        /// <returns>The current value, if the file exists. A new <see cref="T" /> object otherwise.</returns>
        public virtual async Task<T> ReadOrCreateAsync()
        {
            if (FileExists) return await ReadFileAsync();
            return new T();
        }

        /// <summary>
        ///     Persists the data. If the file does not exist, creates a new one.
        /// </summary>
        /// <param name="obj">An object with the data to save.</param>
        public async Task SaveAsync(T obj)
        {
            await SaveToFileAsync(obj);
        }
    }
}