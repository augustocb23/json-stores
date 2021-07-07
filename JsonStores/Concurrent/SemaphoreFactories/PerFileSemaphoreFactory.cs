using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace JsonStores.Concurrent.SemaphoreFactories
{
    /// <summary>
    ///     Uses the file path to create a system-wide semaphore.
    /// </summary>
    /// <remarks>This class is only available on Windows.</remarks>
    public class PerFileSemaphoreFactory : ISemaphoreFactory
    {
        private const string DefaultSemaphorePrefix = "cb96cc39-d2a6-4190-9119-7aaaebfa4443";
        private readonly JsonStoreOptions _options;
        private readonly string _semaphorePrefix;

        private IDictionary<string, Semaphore> _semaphores;

        /// <summary>
        ///     Creates a new instance using the default semaphore name prefix and default options.
        /// </summary>
        /// <seealso cref="JsonStoreOptions" />
        public PerFileSemaphoreFactory() : this(new JsonStoreOptions())
        {
            ThrowIfIsNotWindows();
        }

        /// <summary>
        ///     Creates a new instance using the default semaphore name prefix and the specified <see cref="JsonStoreOptions" />.
        /// </summary>
        /// <param name="options">The options used to retrieve the file name.</param>
        public PerFileSemaphoreFactory(JsonStoreOptions options)
        {
            ThrowIfIsNotWindows();

            _options = options;
            _semaphorePrefix = DefaultSemaphorePrefix;
        }

        /// <summary>
        ///     Creates a new instance specifying the options and the prefix to be used on named semaphores.
        /// </summary>
        /// <param name="options">The options used to retrieve the file name.</param>
        /// <param name="semaphorePrefix">The prefix to be used on named semaphores.</param>
        public PerFileSemaphoreFactory(JsonStoreOptions options, string semaphorePrefix)
        {
            ThrowIfIsNotWindows();

            _options = options;
            _semaphorePrefix = semaphorePrefix;
        }

        public Semaphore GetSemaphore<T>()
        {
            _semaphores ??= new ConcurrentDictionary<string, Semaphore>();

            // get the file name, replacing directory separators since named semaphores can't have it
            var fileName = _options.GetFileFullPath<T>()
                .Replace(Path.DirectorySeparatorChar.ToString(), ";");
            if (_semaphores.ContainsKey(fileName))
                return _semaphores[fileName];

            var semaphore = new Semaphore(1, 1, $"{_semaphorePrefix};{fileName}");
            _semaphores.Add(fileName, semaphore);

            return semaphore;
        }

        public void Dispose()
        {
            if (_semaphores == null) return;

            foreach (var semaphore in _semaphores.Values)
                semaphore.Dispose();
        }

        private static void ThrowIfIsNotWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException(
                    $"'{nameof(PerFileSemaphoreFactory)}' is only available on Windows.");
        }
    }
}