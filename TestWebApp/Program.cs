using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDb.Asp.ConfigurationProvider;

namespace TestWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureMongoConfiguration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void ConfigureMongoConfiguration(HostBuilderContext arg1, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForAllKeysAllDocuments(
                @"mongodb://<CONNECTION_STRING>",
                "myconfigdb", "settings"));

            //configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForDefinedKeysAllDocuments(
            //    @"mongodb://<CONNECTION_STRING>",
            //    "myconfigdb", "settings","key2","key3"));

            //configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForAllKeysFilteredDocuments(
            //    @"mongodb://<CONNECTION_STRING>",
            //    "myconfigdb", "settings", "environment", "qa"));
            configurationBuilder.AddMongoDbConfiguration(MongoDbConfigOptions.GetOptionsForDefinedKeysFilteredDocument(
                @"mongodb://<CONNECTION_STRING>",
                "myconfigdb", "settings", "environment", "dev", "key1", "key2", "key9"));
        }
    }
}
