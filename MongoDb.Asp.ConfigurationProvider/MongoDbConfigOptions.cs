namespace MongoDb.Asp.ConfigurationProvider
{
    public sealed class MongoDbConfigOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string[] KeysToRead { get; set; }
        public ConfigReadOption ReadOption { get; set; }
        public bool QueryInFilteredMode { get; set; }
        public string KeyToQuery { get; set; }
        public object ValueToMatch { get; set; }

        public bool LiveReload { get; set; }

        private MongoDbConfigOptions(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            ReadOption = ConfigReadOption.ReadAll;
        }

        private MongoDbConfigOptions(string connectionString, string databaseName, string collectionName, string[] keysToRead)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            KeysToRead = keysToRead;
            ReadOption = ConfigReadOption.DefinedKeys;
        }

        /// <summary>
        /// Creates the default configuration to read from a given mongo db collection for ALL Keys. It will traverse all documents to extract keys. Keys can be OVERWRITTEN
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>MongoDbConfigOptions.</returns>
        public static MongoDbConfigOptions GetOptionsForAllKeysAllDocuments(string connectionString, string databaseName, string collectionName)
        {
            return new MongoDbConfigOptions(connectionString, databaseName, collectionName);
        }

        /// <summary>
        /// Creates the default configuration to read from a given mongo db collection for DEFINED Keys. It will traverse all documents to extract keys. Keys can be OVERWRITTEN
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="keysToRead">The keys to read.</param>
        /// <returns>MongoDbConfigOptions.</returns>
        public static MongoDbConfigOptions GetOptionsForDefinedKeysAllDocuments(string connectionString, string databaseName, string collectionName, params string[] keysToRead)
        {
            return new MongoDbConfigOptions(connectionString, databaseName, collectionName, keysToRead);
        }

        /// <summary>
        /// Creates the default configuration to read from a given mongo db collection for ALL Keys  additionally applying a filter to fetch records. Keys can be OVERWRITTEN if multiple records are found for the filter.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="keyToQuery">The key to query.</param>
        /// <param name="valueToMatch">The value to match.</param>
        /// <returns>MongoDbConfigOptions.</returns>
        public static MongoDbConfigOptions GetOptionsForAllKeysFilteredDocuments(string connectionString, string databaseName,
            string collectionName, string keyToQuery, object valueToMatch)
        {
            var instance = new MongoDbConfigOptions(connectionString, databaseName, collectionName)
            {
                QueryInFilteredMode = true,
                KeyToQuery = keyToQuery,
                ValueToMatch = valueToMatch
            };
            return instance;
        }
        /// <summary>
        /// Creates the default configuration to read from a given mongo db collection for DEFINED Keys additionally applying a filter to fetch records.Keys can be OVERWRITTEN if multiple records are found for the filter.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="keyToQuery">The key to query.</param>
        /// <param name="valueToMatch">The value to match.</param>
        /// <param name="keysToRead">The keys to read.</param>
        /// <returns>MongoDbConfigOptions.</returns>
        public static MongoDbConfigOptions GetOptionsForDefinedKeysFilteredDocument(string connectionString, string databaseName,
            string collectionName, string keyToQuery, object valueToMatch, params string[] keysToRead)
        {
            var instance = new MongoDbConfigOptions(connectionString, databaseName, collectionName, keysToRead)
            {
                QueryInFilteredMode = true,
                KeyToQuery = keyToQuery,
                ValueToMatch = valueToMatch
            };
            return instance;
        }
    }
}
