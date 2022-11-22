using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BookApi.DataAccess
{
    public class MyListDAL : BaseDAL<MyList>, IMyListDAL
    {
        public MyListDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "myList")
        { }

        public override async Task<IEnumerable<MyList>> GetAll()
        {
            var filter = FilterBuilder.Eq(m => m.Username, _username);
            return (await _collections.Aggregate()
                                      .Match(filter)
                                      .Lookup("books", "BookId", "_id", "Book")
                                      .Unwind("Book")
                                      .As<MyList>()
                                      .ToListAsync());
        }

        public async Task<bool> AddItemToMyList(string itemId)
        {
            var objectId = GetObjectId(itemId);
            var filterDefinition = FilterBuilder.Where(f => f.BookId == objectId.ToString() && f.Username == _username);
            var myList = await Get(filterDefinition);

            if (myList != null)
            {
                var updateDefinition = UpdateBuilder.Set(u => u.Quantity, myList.Quantity + 1);
                await Update(filterDefinition, updateDefinition);
                return true;
            }
            else
            {
                myList = new MyList
                {
                    BookId = itemId,
                    Username = _username,
                    Quantity = 1
                };
                await Save(myList);
                return true;
            }
        }

        public async Task<bool> CheckItemExistsInMyList(string itemId)
        {
            var objectId = GetObjectId(itemId);
            var filterDefinition = FilterBuilder.Where(f => f.BookId == objectId.ToString() && f.Username == _username);
            var result = await CountDocumentsAsync(filterDefinition);
            return result == 0 ? false : true;
        }

        public async Task<bool> DeleteItemFromMyList(string itemId)
        {
            var filterDefinition = FilterBuilder.Where(f => f.BookId == itemId && f.Username == _username);
            await RemoveOne(filterDefinition);
            return true;
        }

        public async Task<int> GetListCounts() => await _collections.AsQueryable()
                .Where(c => c.Username == _username).Select(x => x.Quantity)
                .SumAsync(d => d);

        public override Task<bool> Remove(IEnumerable<string> ids)
        {
            var filterDefinition = FilterBuilder.In(f => f.BookId, ids);
            return RemoveMany(filterDefinition);
        }
    }
}