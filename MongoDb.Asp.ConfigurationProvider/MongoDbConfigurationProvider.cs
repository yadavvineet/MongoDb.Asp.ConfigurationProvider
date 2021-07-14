using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly ILogger _logger;

        private Task _configurationListeningTask;

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
            _itemsCollection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
        public IDictionary<string, string> GetData(BsonDocument doc)
        {
            var json = doc.ToJson();
            var o = JsonConfigurationFileParser.Parse(GenerateStreamFromString(json));
            return o;
        }
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        void AddDocumentToDictionary(BsonDocument document)
        {
            document.Remove("_id");
            if (_readMode == ConfigReadOption.DefinedKeys && _keysToRead.Length > 0)
            {
                //remove any key listed in _keysToRead
                foreach (var name in document.Names.ToList())
                {
                    if (!_keysToRead.Any(b => b.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        document.Remove(name);
                    }
                }
            }
            foreach (var item in GetData(document))
            {
                _itemsCollection.Add(item.Key, item.Value);
            }
            
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
                    AddDocumentToDictionary(document);
                }
            }
            else if (_readMode == ConfigReadOption.DefinedKeys && _keysToRead.Length > 0)
            {
                foreach (var document in configItemList)
                {
                    AddDocumentToDictionary(document);
                }
            }
            
            _configurationListeningTask = new Task(WatchChanges);
            _configurationListeningTask.Start();
        }

        private async void WatchChanges()
        {
            var mongoClient = new MongoClient(_connectionString);
            var mongoServer = mongoClient.GetDatabase(_database);
            var collection = mongoServer.GetCollection<BsonDocument>(_collectionToUse);

            try
            {
                using (var cursor = await collection.WatchAsync())
                {
                    // Read complete change to avoid reloading multiple times
                    while (await cursor.MoveNextAsync() && !cursor.Current.Any())
                    {
                    }
                    
                    Load();
                    _token.OnReload();
                }
            }
            catch (MongoCommandException)
            {
                Console.WriteLine("Change streams not available for configured MongoDb, disabling LiveReload.");   
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
        /// Returns the list of keys that this provider has.
        /// </summary>
        /// <param name="earlierKeys">The earlier keys that other providers contain.</param>
        /// <param name="parentPath">The path for the parent IConfiguration.</param>
        /// <returns>The list of keys for this provider.</returns>
        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return _itemsCollection
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }
    }
}
