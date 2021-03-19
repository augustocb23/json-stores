namespace JsonStores.NamingStrategies
{
    /// <summary>
    ///     Uses the name of the class as the name for the file.
    /// </summary>
    public class ClassNameNamingStrategy : INamingStrategy
    {
        /// <inheritdoc />
        public string GetName<T>() => typeof(T).Name;
    }
}