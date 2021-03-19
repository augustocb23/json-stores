using System;

namespace JsonStores.Annotations
{
    /// <summary>
    ///     When looking for an <see cref="IJsonRepository{T,TKey}"/> key, ignore the Id property, if any.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreIdProperty : Attribute
    {
    }
}