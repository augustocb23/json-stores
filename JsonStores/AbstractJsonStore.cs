using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JsonStores.Exceptions;

namespace JsonStores
{
    /// <summary>
    ///     An abstract implementation to persist data on JSON format.
    /// </summary>
    /// <typeparam name="T">The type used to serialize and deserialize data.</typeparam>
    public abstract class AbstractJsonStore<T> where T : class
    {
        private readonly JsonStoreOptions _options;
        private string _fileName;

        protected AbstractJsonStore(JsonStoreOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = new JsonStoreOptions(options);
        }

        /// <summary>
        ///     A flag indicating if the file exists.
        /// </summary>
        protected bool FileExists => File.Exists(FileFullPath);

        /// <summary>
        ///     Last time the content was loaded or persisted.
        /// </summary>
        private DateTime LastUpdate { get; set; }

        /// <summary>
        ///     A flag indicating if the file was changed since last reload.
        /// </summary>
        /// <remarks>Always returns <c>true</c> if the data was not loaded.</remarks>
        protected bool FileChanged
        {
            get
            {
                var fileInfo = GetFileInfo();
                return fileInfo.Exists && fileInfo.LastWriteTime > LastUpdate;
            }
        }

        /// <summary>
        ///     A <see cref="System.IO.FileInfo" /> for the file.
        /// </summary>
        /// <remarks>Always creates a new instance to ensure that the data is up to date.</remarks>
        private FileInfo GetFileInfo() => new FileInfo(FileFullPath);

        /// <summary>
        ///     Full file path to the file.
        /// </summary>
        private string FileFullPath => _options.GetFileFullPath(FileName);

        /// <summary>
        ///     The name of the file to persist the data, without the extension.
        /// </summary>
        protected virtual string FileName => _fileName ??= _options.NamingStrategy.GetName<T>();

        /// <summary>
        ///     Asynchronously persist data to file. If the file does not exist, it will be created.
        /// </summary>
        /// <param name="content">The data to persist.</param>
        /// <exception cref="FileChangedException">File was changed since the last reload.</exception>
        /// <remarks>All existing data will be override.</remarks>
        /// <seealso cref="JsonStoreOptions.ThrowOnSavingChangedFile"/>
        protected async Task SaveToFileAsync(T content)
        {
            // if the object was previously loaded, check for changes
            if (_options.ThrowOnSavingChangedFile &&
                LastUpdate != DateTime.MinValue && FileChanged)
                throw new FileChangedException(FileFullPath);

            Directory.CreateDirectory(GetFileInfo().DirectoryName!);

            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            await File.WriteAllBytesAsync(FileFullPath, jsonContent);
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        ///     Asynchronously read data. The file must exist.
        /// </summary>
        /// <exception cref="ApplicationException">An error occurred deserializing the file.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <exception cref="IOException">The file is open by another process.</exception>
        protected async Task<T> ReadFileAsync()
        {
            try
            {
                await using var readStream = File.OpenRead(FileFullPath);
                var content = await JsonSerializer.DeserializeAsync<T>(readStream);

                LastUpdate = DateTime.Now;
                return content;
            }
            catch (JsonException e)
            {
                throw new ApplicationException($"There is a problem with file '{FileFullPath}': {e.Message}");
            }
        }
    }
}