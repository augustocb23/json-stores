using System.Threading.Tasks;

namespace JsonStores
{
    public interface IJsonStore<T> where T : class, new()
    {
        /// <summary>
        ///     Reads the configuration file, if exists, and return its value.
        /// </summary>
        /// <returns>The current value, if the file exists. <c>Null</c> otherwise.</returns>
        Task<T> ReadAsync();

        /// <summary>
        ///     Reads the configuration file and return its value. If the file don't exists, returns an empty object.
        /// </summary>
        /// <returns>The current value, if the file exists. A new <see cref="T" /> object otherwise.</returns>
        Task<T> ReadOrCreateAsync();

        /// <summary>
        ///     Persists the data. If the file does not exist, creates a new one.
        /// </summary>
        /// <param name="obj">An object with the data to save.</param>
        Task SaveAsync(T obj);
    }
}