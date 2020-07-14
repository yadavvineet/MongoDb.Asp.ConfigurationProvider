using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    /// <summary>Class MongoDbConfigurationSource.
    /// Implements the <see cref="IConfigurationSource"/></summary>
    /// <seealso cref="IConfigurationSource" />
    public class MongoDbConfigurationSource : IConfigurationSource
    {
        private readonly MongoDbConfigOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbConfigurationSource"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MongoDbConfigurationSource(MongoDbConfigOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Builds the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        /// <returns>An <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MongoDbConfigurationProvider(_options);
        }
    }
}