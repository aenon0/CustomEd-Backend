using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CustomEd.Shared.Data
{
    /// <summary>
    /// Provides extension methods for configuring MongoDB in the service collection.
    /// </summary>
    public static class Extensions
    {
        private static MongoSettings? _mongoSettings;
        private static ServiceSettings? _serviceSettings;

        /// <summary>
        /// Adds MongoDB configuration to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                _serviceSettings = configuration!.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;
                _mongoSettings = configuration!.GetSection(nameof(MongoSettings)).Get<MongoSettings>()!;

                var mongoClient = new MongoClient(_mongoSettings!.ConnectionString);
                // Console.WriteLine($"MongoDB connected to {_mongoSettings.ConnectionString}");
                return mongoClient.GetDatabase(_serviceSettings!.ServiceName);
            });

            return services;
        }

        /// <summary>
        /// Adds persistence configuration for a specific entity type to the service collection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddPersistence<T>(this IServiceCollection services, string collectionName) where T : BaseEntity
        {
            services.AddSingleton<IGenericRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                var collection = database!.GetCollection<T>(collectionName);
                return new GenericRepository<T>(database, collection);
            });
            return services;
        }
    }
}