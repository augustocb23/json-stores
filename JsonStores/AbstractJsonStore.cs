using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JsonStores.NamingStrategies;

namespace JsonStores
{
    /// <summary>
    ///     An abstract implementation to persist data on JSON format.
    /// </summary>
    /// <typeparam name="T">The type used to serialize and deserialize data.</typeparam>
    public abstract class AbstractJsonStore<T> where T : class
    {
        protected AbstractJsonStore(JsonStoreOptions options)
        {
            Location = options.Location;
            NamingStrategy = options.NamingStrategy;
            FileExtension = options.FileExtension;
        }

        private INamingStrategy NamingStrategy { get; }

        /// <summary>
        ///     A flag indicating if the file exists.
        /// </summary>
        protected bool FileExists => FileRef.Exists;

        /// <summary>
        ///     Last time the file was written.
        /// </summary>
        private DateTime LastFileWrite => FileRef.LastWriteTime;

        /// <summary>
        ///     Last time the content was loaded or persisted.
        /// </summary>
        private DateTime? LastUpdate { get; set; }

        /// <summary>
        ///     A flag indicating if the file was changed since last reload.
        /// </summary>
        /// <remarks>Will always return <c>false</c> if the data was not loaded.</remarks>
        protected bool FileChanged => FileExists && LastFileWrite > LastUpdate;

        /// <summary>
        ///     The directory to save the file.
        /// </summary>
        private string Location { get; }

        /// <summary>
        ///     A <see cref="FileInfo" /> for the file.
        /// </summary>
        private FileInfo FileRef => new FileInfo($@"{Location}\{FileName}.{FileExtension}");

        private string FileExtension { get; }

        /// <summary>
        ///     The name of the file to persist the data, without the extension.
        /// </summary>
        protected virtual string FileName => NamingStrategy.GetName<T>();

        /// <summary>
        ///     Asynchronously persist data to file. If the file does not exist, it will be created.
        /// </summary>
        /// <param name="content">The data to persist.</param>
        /// <exception cref="InvalidOperationException">File was changed after last reload.</exception>
        /// <remarks>All existing data will be override.</remarks>
        protected async Task SaveToFileAsync(T content)
        {
            if (FileChanged)
                throw new InvalidOperationException("File was changed after last reload.");

            Directory.CreateDirectory(FileRef.DirectoryName!);

            await using var writeStream = FileRef.Create();
            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            await writeStream.WriteAsync(jsonContent);
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        ///     Asynchronously read data. The file must exist.
        /// </summary>
        /// <exception cref="ApplicationException">An error occurred deserializing the file.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <exception cref="JsonException">The file is not in a valid format.</exception>
        protected async Task<T> ReadFileAsync()
        {
            try
            {
                await using var readStream = FileRef.Open(FileMode.Open, FileAccess.Read);
                var content = await JsonSerializer.DeserializeAsync<T>(readStream);

                LastUpdate = DateTime.Now;
                return content;
            }
            catch (JsonException e)
            {
                throw new ApplicationException($"There is a problem with file '{FileRef.FullName}': {e.Message}");
            }
        }
    }
}