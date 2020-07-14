namespace MongoDb.Asp.ConfigurationProvider
{
    public sealed class MongoDbConfigOptions
    {
        internal string ConnectionString { get; }
        internal string DatabaseName { get; }
        internal string CollectionName { get; }
        internal string[] KeysToRead { get; }
        internal ConfigReadOption ReadOption { get; }
        internal bool QueryInFilteredMode { get; set; }
        internal string KeyToQuery { get; set; }
        internal object ValueToMatch { get; set; }

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

        public static MongoDbConfigOptions GetOptionsForAllKeysAllDocuments(string connectionString, string databaseName, string collectionName)
        {
            return new MongoDbConfigOptions(connectionString, databaseName, collectionName);
        }
        public static MongoDbConfigOptions GetOptionsForDefinedKeysAllDocuments(string connectionString, string databaseName, string collectionName, params string[] keysToRead)
        {
            return new MongoDbConfigOptions(connectionString, databaseName, collectionName, keysToRead);
        }
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
