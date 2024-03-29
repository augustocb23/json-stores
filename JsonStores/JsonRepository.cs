﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JsonStores.Exceptions;
using JsonStores.Helpers;

namespace JsonStores
{
    /// <inheritdoc cref="IJsonRepository{T,TKey}" />
    public class JsonRepository<T, TKey> : AbstractJsonStore<ICollection<T>>, IJsonRepository<T, TKey> where T : class
        where TKey : notnull
    {
        /// <summary>
        ///     Creates a new instance with the given options.
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        public JsonRepository(JsonStoreOptions options) : base(options)
        {
            var keyProperty = RepositoryKeyValidator.GetKeyProperty<T, TKey>();
            GetKeyValue = keyProperty.Compile();
        }

        /// <summary>
        ///     Creates a new instance with the given options and key.
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        /// <param name="keyProperty">A <see cref="Func{T,TResult}" /> to get the object's key.</param>
        public JsonRepository(JsonStoreOptions options, Expression<Func<T, TKey>> keyProperty) : base(options)
        {
            GetKeyValue = keyProperty.Compile();
        }

        /// <summary>
        ///     A collections containing the loaded data.
        /// </summary>
        private ICollection<T> Items { get; set; }

        /// <summary>
        ///     Method used to obtain an object's key value.
        /// </summary>
        private Func<T, TKey> GetKeyValue { get; }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items;
        }

        /// <inheritdoc />
        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
        }

        /// <inheritdoc />
        public virtual async Task AddAsync(T item)
        {
            if (await ExistsAsync(GetKeyValue(item)))
                throw new UniquenessConstraintViolationException(item, GetKeyValue(item));

            Items.Add(item);
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync([NotNull] T item)
        {
            if (Items == null || FileChanged) await LoadAsync();

            await RemoveAsync(GetKeyValue(item));
            await AddAsync(item);
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync([NotNull] TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            return Items.Any(item => id.Equals(GetKeyValue(item)));
        }

        /// <inheritdoc />
        public virtual async Task RemoveAsync([NotNull] TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            var obj = Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
            if (obj == null) throw new ItemNotFoundException(id);

            Items.Remove(obj);
        }

        /// <inheritdoc />
        public virtual async Task SaveChangesAsync()
        {
            if (Items == null)
                throw new InvalidOperationException("Content was never loaded.");

            await SaveToFileAsync(Items);
        }

        /// <summary>
        ///     Load (or initialize) current content.
        /// </summary>
        /// <returns>Current content of the collection.</returns>
        private async Task LoadAsync()
        {
            if (FileExists) Items = await ReadFileAsync();
            else Items = new List<T>();
        }
    }
}