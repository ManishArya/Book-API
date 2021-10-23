using BookApi.models;

namespace BookApi.DataAccess
{
    public class GenreDAL : BaseDAL<Genre>
    {
        public GenreDAL(IDatabaseClient client) : base(client, "genres")
        {

        }
    }
}