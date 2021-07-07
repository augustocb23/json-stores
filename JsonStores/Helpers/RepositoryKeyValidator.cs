using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JsonStores.Annotations;
using JsonStores.Exceptions;

namespace JsonStores.Helpers
{
    internal static class RepositoryKeyValidator
    {
        /// <summary>
        ///     Look for an Id property on given type.
        /// </summary>
        /// <returns>A <see cref="Func{TResult}"/> to get the property.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The property <c>Id</c> is from other type then <see cref="TKey"/> - or -
        ///     There is no property with attribute <see cref="JsonRepositoryKey"/> - or -
        ///     There is more then one property with attribute <see cref="JsonRepositoryKey"/>.
        /// </exception>
        public static Expression<Func<T, TKey>> GetKeyProperty<T, TKey>()
        {
            // get key from property Id, if don't has IgnoreIdProperty annotation
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && typeof(T).GetCustomAttribute<IgnoreIdProperty>() == null)
            {
                ValidatePropertyType<T, TKey>(idProperty);
                return t => (TKey) idProperty.GetValue(t);
            }

            // look for a property with the atribute JsonRepositoryId
            var properties = typeof(T).GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(JsonRepositoryKey), false).Any()).ToArray();
            if (!properties.Any())
                throw new InvalidJsonRepositoryKeyException(
                    $"Class '{typeof(T).Name}' does not has a key. Create a property with name 'Id' or use {nameof(JsonRepositoryKey)} attribute.");
            if (properties.Length > 1)
                throw new InvalidJsonRepositoryKeyException(
                    $"Class '{typeof(T).Name}' contains multiple properties with {nameof(JsonRepositoryKey)} attribute.");

            var propertyWithAttribute = properties.First();
            ValidatePropertyType<T, TKey>(propertyWithAttribute);

            return t => (TKey) propertyWithAttribute.GetValue(t);
        }

        private static void ValidatePropertyType<T, TKey>(PropertyInfo property)
        {
            if (property.PropertyType != typeof(TKey))
                throw new InvalidJsonRepositoryKeyException(
                    $"Property '{typeof(T).Name}.{property.Name}' is not from type {typeof(TKey).Name}'.");
        }
    }
}