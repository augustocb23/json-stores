using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JsonStores
{
    public static class DependencyInjection
    {
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