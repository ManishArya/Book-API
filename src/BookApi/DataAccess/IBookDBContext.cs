using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public interface IBookDBContext
    {
        IMongoCollection<TDoucment> Get<TDoucment>(string name);
    }
}