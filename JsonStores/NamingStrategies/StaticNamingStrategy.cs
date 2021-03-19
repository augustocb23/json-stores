namespace JsonStores.NamingStrategies
{
    /// <summary>
    ///     Use a static string as the name of the file.
    /// </summary>
    public class StaticNamingStrategy : INamingStrategy
    {
        private readonly string _fileName;

        /// <summary>
        ///     Creates a new instance of the class with the given file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public StaticNamingStrategy(string fileName)
        {
            _fileName = fileName;
        }

        /// <inheritdoc />
        public string GetName<T>()
        {
            return _fileName;
        }
    }
}