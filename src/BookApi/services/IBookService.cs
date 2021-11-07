using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooks();

        Task<Book> GetBookById(string id);

        Task<bool> AddBook(Book book);

        Task<bool> DeleteBook(string id);
    }
}