using MongoDB.Driver;
using BookApi.models;
using Microsoft.Extensions.Options;
namespace BookApi.DataAccess
{
    public class BookDBContext : IBookDBContext
    {
        private readonly IMongoDatabase _database;

        public BookDBContext(IOptions<BookstoreDatabaseSettings> options)
        {
            var bookstoreDatabaseSettings = options.Value;
            var client = new MongoClient(bookstoreDatabaseSettings.ConnectionString);
            _database = client.GetDatabase(bookstoreDatabaseSettings.DatabaseName);
        }

        public IMongoCollection<TDocument> Get<TDocument>(string name) => _database.GetCollection<TDocument>(name);
    }
}