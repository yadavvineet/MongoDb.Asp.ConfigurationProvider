using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public class MongoDbConfigurationSource : IConfigurationSource
    {
        private readonly MongoDbConfigOptions _options;

        public MongoDbConfigurationSource(MongoDbConfigOptions options)
        {
            _options = options;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MongoDbConfigurationProvider(_options);
        }
    }
}
