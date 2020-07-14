# .Net Core configuration provider for MongoDb

*Please note, I have moved the project to .net core latest version provide. There are some improvements and known issues which are in progress. Please raise an issue in case of any help needed. PRs are welcome*

# Getting Started

1. Install the nuget package: 

   ```c#
   Install-Package MongoDb.Asp.ConfigurationProvider
   ```

   

2. Update the Program.cs file in web app to use the the new configuration provider. 
               

   ```c#
   public static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration(ConfigureMongoConfiguration) //THIS LINE IS ADDED TO ADD DELEGATE
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<Startup>();
                   });
                   
   //USE THE FOLLOWING METHOD TO ADD PROVIDER.
      private static void ConfigureMongoConfiguration(HostBuilderContext arg1, IConfigurationBuilder configurationBuilder)
       {
           configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForAllKeysAllDocuments(
               @"mongodb://<CONNECTION_STRING>",
               "myconfigdb", "settings"));
       }
   ```

3. Mongo Document used for holding configuration / settings

   ```json
   {
       "_id": {
           "$oid": "5f0e087f575f22cece93ac8d"
       },
       "key1": "value1",
       "key9":"val2",
       "environment":"dev",
       "key3":"val3",
       "key2":{
         "a1":"a3"
       }
   }
   ```



**PS: Please refer the provided TestWebApp to view usage.**



## Configuration Builders

Following four methods are provided currently to load different configurations. "_id" is ignored by default.

1. AllKeys - All Documents - (MongoDbConfigOptions.GetOptionsForAllKeysAllDocuments()) - Use this to read all the keys from all the documents in a collection
2. Defined Keys - All Documents (MongoDbConfigOptions.GetOptionsForDefinedKeysAllDocuments)  - Use this method to load specific set of keys (named keys) from all the documents in a collection.
3. All Keys - Filtered Documents  (MongoDbConfigOptions.GetOptionsForAllKeysFilteredDocuments) - Use this method to load all the keys from filtered document. Filter can be applied for a single set of key value pair. Example could be to look for env variables.
4. Defined Keys - Filtered Documents (MongoDbConfigOptions.GetOptionsForDefinedKeysFilteredDocument) - Use this method to load specific set of keys from filtered document. Filter can be applied for a single set of key value pair. Example could be to look for env variables.





## Assumptions / Known Issues

1. "_id" field is ignored by difficult. This is on-purpose.
2. Nested Keys and child sections are currently not supported.



# Examples Usage

 

```c#
       private static void ConfigureMongoConfiguration(HostBuilderContext arg1, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForAllKeysAllDocuments(
                @"mongodb://<CONNECTION_STRING>",
                "myconfigdb", "settings"));
       configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForDefinedKeysAllDocuments(
           @"mongodb://<CONNECTION_STRING>",
           "myconfigdb", "settings","key2","key3"));

        configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForAllKeysFilteredDocuments(
            @"mongodb://<CONNECTION_STRING>",
            "myconfigdb", "settings", "environment", "qa"));
        
        configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForDefinedKeysFilteredDocument(
            @"mongodb://<CONNECTION_STRING>",
            "myconfigdb", "settings", "environment", "dev", "key1", "key2", "key9"));
    }
```