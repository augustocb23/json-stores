using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace JsonStores
{
    /// <summary>
    ///     Extension methods for setting up JsonStores in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        ///      Adds <see cref="IJsonStore{T}"/> and <see cref="IJsonRepository{T,TKey}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">The options for the stores.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonStores([NotNull] this IServiceCollection services,
            JsonStoreOptions options = default, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.TryAddOptions(options);

            services.Add(new ServiceDescriptor(typeof(IJsonStore<>), typeof(JsonStore<>), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IJsonRepository<,>), typeof(JsonRepository<,>), serviceLifetime));

            return services;
        }

        /// <summary>
        ///      Adds <see cref="IJsonStore{T}"/> and <see cref="IJsonRepository{T,TKey}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">An action to define options.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonStores([NotNull] this IServiceCollection services,
            Action<JsonStoreOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            var jsonOptions = new JsonStoreOptions();
            options(jsonOptions);
            return services.AddJsonStores(jsonOptions, serviceLifetime);
        }

        /// <summary>
        ///      Adds <see cref="IJsonStore{T}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">The options for the store.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonStore<T>([NotNull] this IServiceCollection services,
            JsonStoreOptions options = default, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : class, new()
        {
            services.TryAddOptions(options);

            services.Add(new ServiceDescriptor(typeof(IJsonStore<T>), typeof(JsonStore<T>), serviceLifetime));

            return services;
        }

        /// <summary>
        ///      Adds <see cref="IJsonStore{T}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">An action to define options.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonStore<T>([NotNull] this IServiceCollection services,
            Action<JsonStoreOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : class, new()
        {
            var jsonOptions = new JsonStoreOptions();
            options(jsonOptions);
            return services.AddJsonStore<T>(jsonOptions, serviceLifetime);
        }

        /// <summary>
        ///      Adds <see cref="IJsonRepository{T,TKey}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">The options for the store.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonRepository<T, TKey>([NotNull] this IServiceCollection services,
            JsonStoreOptions options = default, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : class, new()
        {
            services.TryAddOptions(options);

            services.Add(new ServiceDescriptor(typeof(IJsonRepository<T, TKey>),
                typeof(JsonRepository<T, TKey>),
                serviceLifetime));

            return services;
        }

        /// <summary>
        ///      Adds <see cref="IJsonRepository{T,TKey}"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">An action to define options.</param>
        /// <param name="serviceLifetime">The lifetime with which to register the store in the container. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The same instance of <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection AddJsonRepository<T, TKey>([NotNull] this IServiceCollection services,
            Action<JsonStoreOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : class, new()
        {
            var jsonOptions = new JsonStoreOptions();
            options(jsonOptions);
            return services.AddJsonRepository<T, TKey>(jsonOptions, serviceLifetime);
        }
        
        private static void TryAddOptions(this IServiceCollection services, JsonStoreOptions options)
        {
            if (services.All(descriptor => descriptor.ServiceType != typeof(JsonStoreOptions)))
                services.Add(new ServiceDescriptor(typeof(JsonStoreOptions), options ?? new JsonStoreOptions()));
        }
    }
}