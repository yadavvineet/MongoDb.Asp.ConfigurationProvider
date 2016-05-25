using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public static class MongoDbConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddMongoDbConfiguration(this IConfigurationBuilder builder, string connectionName, string collectionToUse)
        {
            return builder.Add(new MongoDbConfigurationProvider(connectionName, collectionToUse));
        }
    }
}
