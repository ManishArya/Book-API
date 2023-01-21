using System.Threading.Tasks;
using System.Collections.Generic;
using BookApi.models;

namespace BookApi.DataAccess
{
    public interface IBookDAL : IBaseDAL<Book>
    {
        Task<bool> DeleteBookAsync(IEnumerable<string> ids);

        Task<(IEnumerable<Book>, long)> GetBooksAsync(BookCriteria criteria);
    }
}