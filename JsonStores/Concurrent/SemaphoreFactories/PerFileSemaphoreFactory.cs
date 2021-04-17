using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace JsonStores.Concurrent.SemaphoreFactories
{
    /// <summary>
    ///     Uses the file path to create a system-wide semaphore.
    /// </summary>
    public class PerFileSemaphoreFactory : ISemaphoreFactory
    {
        private IDictionary<string, Semaphore> _semaphores;
        private JsonStoreOptions _options;

        private readonly string _semaphorePrefix;

        /// <summary>
        ///     Creates a new instance using the default semaphore name prefix.
        /// </summary>
        public PerFileSemaphoreFactory()
        {
            const string defaultSemaphorePrefix = "cb96cc39-d2a6-4190-9119-7aaaebfa4443";
            _semaphorePrefix = defaultSemaphorePrefix;
        }

        /// <summary>
        ///     Creates a new instance specifying the prefix to be used on named semaphores. 
        /// </summary>
        /// <param name="semaphorePrefix">The prefix to be used on named semaphores.</param>
        public PerFileSemaphoreFactory(string semaphorePrefix)
        {
            _semaphorePrefix = semaphorePrefix;
        }

        public Semaphore GetSemaphore<T>()
        {
            _options ??= new JsonStoreOptions();
            _semaphores ??= new ConcurrentDictionary<string, Semaphore>();

            var fileName = _options.GetFileFullPath<T>();
            if (_semaphores.ContainsKey(fileName))
                return _semaphores[fileName];

            var semaphore = new Semaphore(1, 1, $"{_semaphorePrefix}/{fileName}");
            _semaphores.Add(fileName, semaphore);

            return semaphore;
        }

        public void Dispose()
        {
            if (_semaphores == null) return;

            foreach (var semaphore in _semaphores.Values)
                semaphore.Dispose();
        }
    }
}