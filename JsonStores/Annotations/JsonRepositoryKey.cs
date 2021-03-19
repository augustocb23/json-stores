using System;

namespace JsonStores.Annotations
{
    /// <summary>
    ///     Use the property as the key for a <see cref="IJsonRepository{T,TKey}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class JsonRepositoryKey : Attribute
    {
    }
}