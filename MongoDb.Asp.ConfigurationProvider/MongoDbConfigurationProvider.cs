using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDb.Asp.ConfigurationProvider
{
    /// <summary>
    /// Class MongoDbConfigurationProvider.
    /// Implements the <see cref="IConfigurationProvider" />
    /// </summary>
    /// <seealso cref="IConfigurationProvider" />
    public class MongoDbConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// The database
        /// </summary>
        private readonly string _database;
        /// <summary>
        /// The collection to use
        /// </summary>
        private readonly string _collectionToUse;
        /// <summary>
        /// The read mode
        /// </summary>
        private readonly ConfigReadOption _readMode;
        /// <summary>
        /// The items collection
        /// </summary>
        private readonly Dictionary<string, string> _itemsCollection;
        /// <summary>
        /// The keys to read
        /// </summary>
        private readonly string[] _keysToRead;
        /// <summary>
        /// The run in query mode
        /// </summary>
        private readonly bool _runInQueryMode;
        /// <summary>
        /// The key to match
        /// </summary>
        private readonly string _keyToMatch;
        /// <summary>
        /// The value to match
        /// </summary>
        private readonly object _valueToMatch;
        /// <summary>
        /// The token
        /// </summary>
        private readonly ConfigurationReloadToken _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbConfigurationProvider"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MongoDbConfigurationProvider(MongoDbConfigOptions options)
        {
            _connectionString = options.ConnectionString;
            _database = options.DatabaseName;
            _collectionToUse = options.CollectionName;
            _readMode = options.ReadOption;
            _keysToRead = options.KeysToRead;
            _itemsCollection = new Dictionary<string, string>();
            _runInQueryMode = options.QueryInFilteredMode;
            _keyToMatch = options.KeyToQuery;
            _valueToMatch = options.ValueToMatch;
            _token = new ConfigurationReloadToken();
        }

        /// <summary>
        /// Tries to get a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if a value for the specified key was found, otherwise <c>false</c>.</returns>
        public bool TryGet(string key, out string value)
        {
            if (_itemsCollection.ContainsKey(key))
            {
                value = _itemsCollection[key];
                return true;
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="NotSupportedException">Setting a key is not supported.</exception>
        public void Set(string key, string value)
        {
            throw new NotSupportedException("Setting a key is not supported.");
        }

        /// <summary>
        /// Returns a change token if this provider supports change tracking, null otherwise.
        /// </summary>
        /// <returns>The change token.</returns>
        public IChangeToken GetReloadToken()
        {
            return _token;
        }

        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />.
        /// </summary>
        public void Load()
        {
            var mongoClient = new MongoClient(_connectionString);
            var mongoServer = mongoClient.GetDatabase(_database);
            var collection = mongoServer.GetCollection<BsonDocument>(_collectionToUse);
            var configItemList = _runInQueryMode
                ? collection.Find(Builders<BsonDocument>.Filter.Eq(_keyToMatch, _valueToMatch)).ToList()
                : collection.Find(new BsonDocument()).ToList();
            _itemsCollection.Clear();
            if (_readMode == ConfigReadOption.ReadAll)
            {
                foreach (var document in configItemList)
                {
                    foreach (var item in document.Names)
                    {
                        if (item.Equals("_id", StringComparison.InvariantCultureIgnoreCase)) continue;

                        if (_itemsCollection.ContainsKey(item))
                        {
                            _itemsCollection[item] = GetStringValue(document.GetValue(item));
                        }
                        else
                        {
                            _itemsCollection.Add(item, GetStringValue(document.GetValue(item)));
                        }
                    }
                }
            }
            else if (_readMode == ConfigReadOption.DefinedKeys && _keysToRead.Length > 0)
            {
                foreach (var document in configItemList)
                {
                    foreach (var item in document.Names)
                    {
                        if (_keysToRead.Any(b => b.Equals(item, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            if (_itemsCollection.ContainsKey(item))
                            {
                                _itemsCollection[item] = GetStringValue(document.GetValue(item));
                            }
                            else
                            {
                                _itemsCollection.Add(item, GetStringValue(document.GetValue(item)));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private string GetStringValue(BsonValue value)
        {
            var doc = value as BsonDocument;
            return doc != null ? doc.ToJson() : value.ToString();
        }

        /// <summary>
        /// Returns the immediate descendant configuration keys for a given parent path based on this
        /// <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />s data and the set of keys returned by all the preceding
        /// <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />s.
        /// </summary>
        /// <param name="earlierKeys">The child keys returned by the preceding providers for the same parent path.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns>The child keys.</returns>
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            return earlierKeys;
        }
    }
}
