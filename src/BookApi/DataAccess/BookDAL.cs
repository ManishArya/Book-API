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
            if (isAdmin)
            {
                return base.Save(book);
            }

            throw new SecurityException("Permission denied");
        }

        public override Task<bool> Remove(string id)
        {
            if (isAdmin)
            {
                return base.Remove(id);
            }

            throw new SecurityException("Permission denied");
        }
    }
}