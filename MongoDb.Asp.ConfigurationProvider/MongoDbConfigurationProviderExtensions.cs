using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public static class MongoDbConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddMongoDbConfiguration(this IConfigurationBuilder builder,
            MongoDbConfigOptions setup)
        {
            return
                builder.Add(new MongoDbConfigurationSource(setup));
        }
    }
}
