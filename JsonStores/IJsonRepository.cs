using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JsonStores.Exceptions;

namespace JsonStores
{
    /// <summary>
    ///     Represents a store to persist a collection of items.
    /// </summary>
    /// <typeparam name="T">The type for the collection.</typeparam>
    /// <typeparam name="TKey">The type for the key used to identify an item in the collection.</typeparam>
    public interface IJsonRepository<T, in TKey> where T : class where TKey : notnull
    {
        /// <summary>
        ///     A flag indicating if an index for keys should be created.
        /// </summary>
        bool UseIndexedKeys { get; }

        /// <summary>
        ///     Get all items on the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}" /> with the items.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        ///     Receives a Id and look for a item with this key.
        /// </summary>
        /// <param name="id">The Id of the item to search.</param>
        /// <returns>The item, if it was found. <c>null</c> otherwise.</returns>
        Task<T> GetByIdAsync(TKey id);

        /// <summary>
        ///     Adds an item to the collection.
        /// </summary>
        /// <param name="item">Item to add to the collection.</param>
        /// <exception cref="UniquenessConstraintViolationException">There is already an item with the same Id.</exception>
        Task AddAsync(T item);

        /// <summary>
        ///     Receives a item and change it.
        /// </summary>
        /// <param name="item">Item to change.</param>
        /// <exception cref="ItemNotFoundException">The item was not found.</exception>
        Task UpdateAsync([NotNull] T item);

        /// <summary>
        ///     Saves the actual items list to file.
        /// </summary>
        /// <exception cref="InvalidOperationException">File was changed after last reload.</exception>
        /// <exception cref="FileChangedException">File was changed since the last reload.</exception>
        /// <seealso cref="JsonStoreOptions.ThrowOnSavingChangedFile" />
        /// <exception cref="InvalidOperationException">The content was never loaded.</exception>
        Task SaveChangesAsync();

        /// <summary>
        ///     Evaluates whether the collection already contains an item with the given Id.
        /// </summary>
        /// <param name="id">Id to looking for.</param>
        /// <returns><c>True</c> if the item exists. <c>False</c> otherwise.</returns>
        Task<bool> ExistsAsync([NotNull] TKey id);

        /// <summary>
        ///     Removes the item with the given Id.
        /// </summary>
        /// <param name="id">Id og the item to remove.</param>
        /// <exception cref="ItemNotFoundException">The item was not found.</exception>
        Task RemoveAsync([NotNull] TKey id);
    }
}