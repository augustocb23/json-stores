using System.IO;
using JsonStores.NamingStrategies;

namespace JsonStores
{
    public class JsonStoreOptions
    {
        /// <summary>
        ///     Creates a new instance with default options.
        /// </summary>
        public JsonStoreOptions()
        {
        }

        /// <summary>
        ///     Creates a new instance from another instance (shallow copy).
        /// </summary>
        /// <param name="options">The instance to use as template.</param>
        public JsonStoreOptions(JsonStoreOptions options)
        {
            Location = options.Location;
            NamingStrategy = options.NamingStrategy;
            FileExtension = options.FileExtension;
        }

        /// <summary>
        ///     The directory where the file will be saved.
        /// </summary>
        public string Location { get; set; } = Directory.GetCurrentDirectory();

        /// <summary>
        ///     The strategy used to get the file name.
        /// </summary>
        public INamingStrategy NamingStrategy { get; set; } = new ClassNameNamingStrategy();

        /// <summary>
        ///     The extension for the JSON file.
        /// </summary>
        public string FileExtension { get; set; } = "json";
    }
}