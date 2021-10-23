using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;

namespace BookApi.services
{
    public class BookService : IBookService
    {
        private readonly IBaseDAL<Book> _bookDAL;
        public BookService(IBaseDAL<Book> bookDAL) => _bookDAL = bookDAL;

        public async Task<IEnumerable<Book>> GetBooks() => await _bookDAL.GetAll();

        public async Task<string> AddBook(Book book) => await _bookDAL.Save(book);

        public async Task<Book> GetBookById(string id) => await _bookDAL.GetById(id);

        public async Task<string> DeleteBook(string id) => await _bookDAL.Remove(id);
    }
}