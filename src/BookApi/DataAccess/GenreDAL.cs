using BookApi.models;
using Microsoft.AspNetCore.Http;

namespace BookApi.DataAccess
{
    public class GenreDAL : BaseDAL<Genre>
    {
        public GenreDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "genres")
        { }
    }
}