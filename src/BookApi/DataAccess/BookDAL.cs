using BookApi.models;
using Microsoft.AspNetCore.Http;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>
    {
        public BookDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "books")
        {

        }
    }
}