using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace JsonStores.Concurrent
{
    /// <summary>
    ///     Represents a thread-safe store to persist a collection of items.
    /// </summary>
    /// <inheritdoc cref="IJsonRepository{T,TKey}"/>
    public interface IConcurrentJsonRepository<T, in TKey> : IJsonRepository<T, TKey>
        where T : class, new() where TKey : notnull
    {
        /// <summary>
        ///     Adds an item and saves the collection.
        /// </summary>
        /// <inheritdoc cref="IJsonRepository{T,TKey}.AddAsync"/>
        Task AddAndSaveAsync(T obj);

        /// <summary>
        ///     Updates an item and saves the collection.
        /// </summary>
        /// <inheritdoc cref="IJsonRepository{T,TKey}.UpdateAsync"/>
        Task UpdateAndSaveAsync([NotNull] T item);

        /// <summary>
        ///     Removes an item and saves the collection.
        /// </summary>
        /// <inheritdoc cref="IJsonRepository{T,TKey}.RemoveAsync"/>
        Task RemoveAndSaveAsync([NotNull] TKey id);
    }
}