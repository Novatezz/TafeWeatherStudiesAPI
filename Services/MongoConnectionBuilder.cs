using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notes.Settings;

namespace Notes.Services
{
    public class MongoConnectionBuilder
    {
        //variable to hold our settings class - dependancy injection
        private readonly IOptions<MongoConnectionSettings> _settings;

        //request our settings class as a parameter in the constuctor - dependancy injection
        public MongoConnectionBuilder(IOptions<MongoConnectionSettings> settings)
        {
            //stored recieved settings
            _settings = settings;
        }

        public IMongoDatabase GetDatabase()
        {
            //Create the class for connecting with the mongo db and hand it our connection string.
            var client = new MongoClient(_settings.Value.ConnectionString);
            //Ask it to connect to our database
            return client.GetDatabase(_settings.Value.DatabaseName);
        }
    }
}
