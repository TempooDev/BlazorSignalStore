using Microsoft.Extensions.DependencyInjection;

namespace BlazorSignalStore
{
    /// <summary>
    /// Extension methods for configuring BlazorSignalStore services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a signal store to the service collection as a scoped service.
        /// </summary>
        /// <typeparam name="TStore">The type of the store to register.</typeparam>
        /// <param name="services">The service collection to add the store to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSignalStore<TStore>(this IServiceCollection services)
            where TStore : class
        {
            services.AddScoped<TStore>();
            return services;
        }
    }
}
