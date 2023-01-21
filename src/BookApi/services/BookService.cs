using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;

namespace BookApi.services
{
    public class BookService : IBookService
    {
        private readonly IBookDAL _bookDAL;

        public BookService(IBookDAL bookDAL) => _bookDAL = bookDAL;

        public async Task<PageResponseApi<IEnumerable<Book>>> GetBooksAsync(BookCriteria criteria)
        {
            var (books, count) = await _bookDAL.GetBooksAsync(criteria);
            return new PageResponseApi<IEnumerable<Book>>(books, count);
        }

        public async Task AddBookAsync(Book book) => await _bookDAL.SaveDocumentAsync(book);

        public async Task<ResponseApi<Book>> GetBookByIdAsync(string id) => new ResponseApi<Book>(await _bookDAL.GetDocumentByIdAsync(id));

        public async Task<BaseResponse> DeleteBooksAsync(ICollection<string> ids)
        {
            await _bookDAL.DeleteBookAsync(ids);
            return new BaseResponse("Books Delete Successfully", HttpStatusCode.OK);
        }
    }
}