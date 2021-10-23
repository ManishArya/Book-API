using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public class MyListDAL : BaseDAL<MyList>, IMyListDAL
    {

        private readonly string _username;

        public MyListDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(client, "myList")
        {
            _username = contextAccessor.HttpContext.User.Identity.Name;
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
            var result = await _collections.CountDocumentsAsync(c => c.BookId == itemId && c.Username == _username);

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