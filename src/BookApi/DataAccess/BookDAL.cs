using BookApi.models;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>
    {
        public BookDAL(IDatabaseClient client) : base(client, "books")
        {

        }
    }
}