namespace JsonStores.Concurrent
{
    /// <summary>
    ///     Represents a thread-safe store to persist a single item.  
    /// </summary>
    /// <inheritdoc cref="IJsonStore{T}"/>
    public interface IConcurrentJsonStore<T> : IJsonStore<T> where T : class, new()
    {
    }
}