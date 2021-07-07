using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace JsonStores.Concurrent.SemaphoreFactories
{
    /// <summary>
    ///     Creates a named (system-wide) semaphore.
    /// </summary>
    /// <remarks>This class is only available on Windows.</remarks>
    public class NamedSemaphoreFactory : ISemaphoreFactory
    {
        private readonly string _semaphoreName;
        private Semaphore _semaphore;

        /// <summary>
        ///     Creates a new instance using the specified semaphore name.
        /// </summary>
        /// <param name="semaphoreName">The name to be used in the semaphore.</param>
        public NamedSemaphoreFactory(string semaphoreName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException(
                    $"'{nameof(NamedSemaphoreFactory)}' is only available on Windows.");

            _semaphoreName = semaphoreName;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets a singleton named semaphore instance. The <typeparamref name="T" /> param is ignored.
        /// </summary>
        public Semaphore GetSemaphore<T>()
        {
            return _semaphore ??= new Semaphore(1, 1, _semaphoreName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _semaphore?.Dispose();
        }
    }
}