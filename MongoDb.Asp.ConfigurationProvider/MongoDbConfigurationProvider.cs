using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDb.Asp.ConfigurationProvider
{
    public class MongoDbConfigurationProvider : IConfigurationProvider
    {
        private readonly string _databaseConnection;
        private readonly string _database;
        private readonly string _collectionToUse;
        private readonly string _columnKey;
        private readonly string _columnValue;
        Dictionary<string, string> itemsCollection;

        public MongoDbConfigurationProvider(string databaseConnection, string database, 
            string collectionToUse, string columnKey, string columnValue)
        {
            _databaseConnection = databaseConnection;
            _database = database;
            _collectionToUse = collectionToUse;
            _columnKey = columnKey;
            _columnValue = columnValue;
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
            if (itemsCollection.ContainsKey(key))
            {
                value =  itemsCollection[key];
                return true;
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param><param name="value">The value.</param>
        public void Set(string key, string value)
        {
            throw new NotSupportedException("Setting a key is not supported.");
        }

        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>.
        /// </summary>
        public void Load()
        {
            var mongoClient = new MongoClient(_databaseConnection);
            var mongoServer = mongoClient.GetDatabase(_database);
            var collection = mongoServer.GetCollection<BsonDocument>(_collectionToUse);
            var configItemList = collection.Find(new BsonDocument()).ToList();
            itemsCollection = new Dictionary<string, string>();
            foreach (var item in configItemList)
            {
                if (item.Names.Contains(_columnKey))
                {
                    if (item.Names.Contains(_columnValue))
                        itemsCollection.Add(item[_columnKey].ToString(), item[_columnValue].ToString());
                    else
                    {
                        itemsCollection.Add(item[_columnKey].ToString(), null);
                    }
                }
            }
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
            throw new NotSupportedException();
        }
    }
}
