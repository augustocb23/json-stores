using System.IO;
using JsonStores.Exceptions;
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
            ThrowOnSavingChangedFile = options.ThrowOnSavingChangedFile;
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

        /// <summary>
        ///     A flag indicating if should look at <see cref="FileInfo.LastWriteTime"/> property when saving after reading the file.
        /// </summary>
        /// <remarks>
        ///     Throws a <see cref="FileChangedException"/> if the file has been changed after loading the content.
        /// </remarks>
        public bool ThrowOnSavingChangedFile { get; set; } = true;

        /// <summary>
        ///     Gets full path to the file for specified file name.
        /// </summary>
        /// <param name="fileName">The name to the file.</param>
        /// <returns>Full path to the file.</returns>
        public string GetFileFullPath(string fileName)
        {
            return Path.Combine(Location, $"{fileName}.{FileExtension}");
        }
    }
}