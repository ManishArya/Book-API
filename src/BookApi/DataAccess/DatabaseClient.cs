using BookApi.models;
using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public class DatabaseClient : IDatabaseClient
    {
        private readonly IMongoDatabase _database;
        public DatabaseClient(IBookstoreDatabaseSettings bookstoreDatabaseSettings)
        {
            var client = new MongoClient(bookstoreDatabaseSettings.ConnectionString);
            _database = client.GetDatabase(bookstoreDatabaseSettings.DatabaseName);
        }

        public IMongoDatabase Database { get => _database; }
    }
}