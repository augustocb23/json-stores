using System.Threading;

namespace JsonStores.Concurrent.SemaphoreFactories
{
    /// <summary>
    ///     A factory to create <see cref="Semaphore" /> instances.
    /// </summary>
    public interface ISemaphoreFactory
    {
        /// <summary>
        ///     Gets a <see cref="Semaphore" /> instance for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to create the instance.</typeparam>
        /// <returns>The semaphore instance.</returns>
        Semaphore GetSemaphore<T>();
    }
}