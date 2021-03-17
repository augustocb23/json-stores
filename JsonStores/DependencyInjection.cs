using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JsonStores
{
    /// <summary>
    ///     Extension methods for setting up JsonStores in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        ///      Adds <see cref="JsonStore{T}"/> and <see cref="JsonRepository{T,TKey}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">The options for the stores.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the DbContext service in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJsonStores([NotNull] this IServiceCollection services,
            JsonStoreOptions options = default, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.TryAdd(new ServiceDescriptor(typeof(JsonStoreOptions), options ?? new JsonStoreOptions()));

            services.TryAdd(new ServiceDescriptor(typeof(IJsonStore<>), typeof(JsonStore<>), serviceLifetime));
            services.TryAdd(
                new ServiceDescriptor(typeof(IJsonRepository<,>), typeof(JsonRepository<,>), serviceLifetime));

            return services;
        }
    }
}