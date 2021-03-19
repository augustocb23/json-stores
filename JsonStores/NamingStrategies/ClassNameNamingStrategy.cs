namespace JsonStores.NamingStrategies
{
    /// <summary>
    ///     Uses the name of the class as the name for the file.
    /// </summary>
    public class ClassNameNamingStrategy : INamingStrategy
    {
        /// <inheritdoc />
        public string GetName<T>()
        {
            // if is a generic type, use the first parameter
            return typeof(T).IsGenericType
                ? typeof(T).GenericTypeArguments[0].Name
                : typeof(T).Name;
        }
    }
}