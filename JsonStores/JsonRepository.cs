using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using JsonStores.Annotations;
using JsonStores.Exceptions;

namespace JsonStores
{
    /// <summary>
    ///     An implementation for <see cref="AbstractJsonStore{T}" /> to store a collection of items.
    /// </summary>
    /// <typeparam name="T">The type for the collection.</typeparam>
    /// <typeparam name="TKey">The type for the key used to identify a item in the collection.</typeparam>
    internal class JsonRepository<T, TKey> : AbstractJsonStore<ICollection<T>>, IJsonRepository<T, TKey> where T : class
        where TKey : notnull
    {
        /// <summary>
        ///     Creates a new instance of <see cref="JsonRepository{T,TKey}"/> with the given options. 
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        public JsonRepository(JsonStoreOptions options) : base(options)
        {
            Key = GetKeyProperty();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="JsonRepository{T,TKey}"/> with the given options and key. 
        /// </summary>
        /// <param name="options">The options for this repository.</param>
        /// <param name="key">A <see cref="Func{T,TKey}"/> to get the object's key.</param>
        public JsonRepository(JsonStoreOptions options, Expression<Func<T, TKey>> key) : base(options)
        {
            Key = key;
        }

        /// <summary>
        ///     Look for an Id property on given type.
        /// </summary>
        /// <returns>A <see cref="Func{T,TKey}"/> to get the property.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The property <c>Id</c> is from other type then <see cref="TKey"/> - or -
        ///     There is no property with attribute <see cref="JsonRepositoryId"/> - or -
        ///     There is more then one property with attribute <see cref="JsonRepositoryId"/>.
        /// </exception>
        private static Expression<Func<T, TKey>> GetKeyProperty()
        {
            // get key from property Id 
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                ValidatePropertyType(idProperty);
                return t => (TKey) idProperty.GetValue(t);
            }

            // look for a property with the atribute JsonRepositoryId
            var properties = typeof(T).GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(JsonRepositoryId), false).Any()).ToArray();
            if (!properties.Any())
                throw new InvalidOperationException(
                    $"Class '{typeof(T).Name}' does not has a key. Create a property with name 'Id' or use {nameof(JsonRepositoryId)} attribute.");
            if (properties.Length > 1)
                throw new InvalidOperationException(
                    $"Class '{typeof(T).Name}' contains multiple properties with {nameof(JsonRepositoryId)} attribute.");

            var propertyWithAttribute = properties.First();
            ValidatePropertyType(propertyWithAttribute);

            return t => (TKey) propertyWithAttribute.GetValue(t);
        }

        private static void ValidatePropertyType(PropertyInfo property)
        {
            if (property.PropertyType != typeof(TKey))
                throw new InvalidOperationException(
                    $"Property '{typeof(T).Name}.{property.Name}' is not from type {typeof(TKey).Name}'.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items;
        }

        /// <inheritdoc />
        public async Task<T> GetByIdAsync(TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();
            return Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
        }

        /// <inheritdoc />
        public async Task AddAsync(T obj)
        {
            if (await ExistsAsync(GetKeyValue(obj)))
                throw new UniquenessConstraintViolationException(obj, GetKeyValue(obj));

            Items.Add(obj);
        }

        /// <inheritdoc />
        public async Task UpdateAsync([NotNull] T item)
        {
            if (Items == null || FileChanged) await LoadAsync();

            await RemoveAsync(GetKeyValue(item));
            await AddAsync(item);
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await SaveToFileAsync(Items);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync([NotNull] TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            return Items.Any(item => id.Equals(GetKeyValue(item)));
        }

        /// <inheritdoc />
        public async Task RemoveAsync([NotNull] TKey id)
        {
            if (Items == null || FileChanged) await LoadAsync();

            var obj = Items.SingleOrDefault(item => id.Equals(GetKeyValue(item)));
            if (obj == null) throw new ItemNotFoundException(id);

            Items.Remove(obj);
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

        /// <summary>
        ///     A collections contained the loaded data.
        /// </summary>
        private ICollection<T> Items { get; set; }

        /// <summary>
        ///     A property that contains a key that identifies a unique item.
        /// </summary>
        private Expression<Func<T, TKey>> Key { get; }

        /// <summary>
        ///     Method used to obtain an object's key value.
        /// </summary>
        private Func<T, TKey> GetKeyValue => Key.Compile();
    }
}