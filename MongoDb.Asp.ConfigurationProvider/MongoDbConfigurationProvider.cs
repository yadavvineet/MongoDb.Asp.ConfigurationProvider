using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MongoDb.Asp.ConfigurationProvider
{
    public class MongoDbConfigurationProvider : IConfigurationProvider
    {

        public MongoDbConfigurationProvider(string databaseConnection, string collectionToUse)
        {
            
        }
        /// <summary>
        /// Tries to get a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param><param name="value">The value.</param>
        /// <returns>
        /// <c>True</c> if a value for the specified key was found, otherwise <c>false</c>.
        /// </returns>
        public bool TryGet(string key, out string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param><param name="value">The value.</param>
        public void Set(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>.
        /// </summary>
        public void Load()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the immediate descendant configuration keys for a given parent path based on this
        ///             <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>'s data and the set of keys returned by all the preceding
        ///             <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>s.
        /// </summary>
        /// <param name="earlierKeys">The child keys returned by the preceding providers for the same parent path.</param><param name="parentPath">The parent path.</param><param name="delimiter">The delimiter to use to identify keys in the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>'s data.</param>
        /// <returns>
        /// The child keys.
        /// </returns>
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath, string delimiter)
        {
            throw new NotImplementedException();
        }
    }
}
