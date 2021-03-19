namespace JsonStores.NamingStrategies
{
    /// <summary>
    ///     The strategy used to get the file name.
    /// </summary>
    public interface INamingStrategy
    {
        /// <summary>
        ///     Gets a string representing the name of the file to persist the store. 
        /// </summary>
        /// <typeparam name="T">The generic type of the store.</typeparam>
        /// <returns>The name of the file, without extension.</returns>
        string GetName<T>();
    }
}