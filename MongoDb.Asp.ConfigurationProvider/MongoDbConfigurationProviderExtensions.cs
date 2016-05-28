using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public static class MongoDbConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddMongoDbConfiguration(this IConfigurationBuilder builder,
            string connectionName, string database, string collectionToUse, string settingsKeyColumn, string settingValueColumn)
        {
            return
                builder.Add(new MongoDbConfigurationProvider(connectionName, database, collectionToUse,
                    settingsKeyColumn, settingValueColumn));
        }
    }
}
