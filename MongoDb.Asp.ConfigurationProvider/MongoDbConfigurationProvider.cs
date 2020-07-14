using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDb.Asp.ConfigurationProvider
{
    public class MongoDbConfigurationProvider : IConfigurationProvider
    {
        private readonly string _connectionString;
        private readonly string _database;
        private readonly string _collectionToUse;
        private readonly ConfigReadOption _readMode;
        private readonly Dictionary<string, string> _itemsCollection;
        private readonly string[] _keysToRead;
        private readonly bool _runInQueryMode;
        private readonly string _keyToMatch;
        private readonly object _valueToMatch;

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
        /// <param name="key">The key.</param><param name="value">The value.</param>
        public void Set(string key, string value)
        {
            throw new NotSupportedException("Setting a key is not supported.");
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider"/>.
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
            else if (_readMode == ConfigReadOption.ReadAll && _keysToRead.Length > 0)
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

        private string GetStringValue(BsonValue value)
        {
            var doc = value as BsonDocument;
            return doc != null ? doc.ToJson() : value.ToString();
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            throw new NotImplementedException();
        }
    }
}
