using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public static class MongoDbConfigurationProviderExtensions
    {
        /// <summary>
        /// Adds the mongo database configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="setup">The setup.</param>
        /// <returns>IConfigurationBuilder.</returns>
        public static IConfigurationBuilder AddMongoDbConfiguration(this IConfigurationBuilder builder,
            MongoDbConfigOptions setup)
        {
            return
                builder.Add(new MongoDbConfigurationSource(setup));
        }
    }
}
