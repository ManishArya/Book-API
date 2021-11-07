using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public class MyListDAL : BaseDAL<MyList>, IMyListDAL
    {
        public MyListDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "myList")
        {
        }

        public override async Task<IEnumerable<MyList>> GetAll()
        {
            var filter = Builders<MyList>.Filter.Eq(m => m.Username, _username);

            return (await _collections.Aggregate().
                               Match(filter)
                            .Lookup("books", "BookId", "_id", "Books")
                                   .As<MyList>().ToListAsync());
        }

        public async Task<bool> AddItemToMyList(string itemId)
        {
            var myList = new MyList();
            myList.BookId = itemId;
            myList.Username = _username;

            await base.Save(myList);
            return true;
        }

        public async Task<bool> CheckItemExistsInMyList(string itemId)
        {
            ObjectId objectId = GetObjectId(itemId);

            var result = await _collections.CountDocumentsAsync(c => c.BookId == objectId.ToString() && c.Username == _username);

            if (result == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteItemFromMyList(string itemId)
        {
            await _collections.DeleteOneAsync(c => c.BookId == itemId);
            return true;
        }
    }
}