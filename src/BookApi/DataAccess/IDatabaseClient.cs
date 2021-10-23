using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public interface IDatabaseClient
    {
        public IMongoDatabase Database { get; }
    }
}