using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>
    {
        public BookDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "books")
        {

        }

        public override Task<bool> Save(Book book)
        {
            if (_isAdmin)
            {
                return base.Save(book);
            }

            throw new SecurityException("Permission denied");
        }

        public override Task<bool> Remove(IEnumerable<string> ids)
        {
            if (_isAdmin)
            {
                var filterDefinition = FilterBuilder.In(f => f.Id, ids);
                return RemoveMany(filterDefinition);
            }

            throw new SecurityException("Permission denied");
        }
    }
}