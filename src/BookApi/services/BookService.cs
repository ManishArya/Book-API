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

        public async Task<ResponseApi<IEnumerable<Book>>> GetBooksAsync() => new ResponseApi<IEnumerable<Book>>(await _bookDAL.GetAsync());

        public async Task AddBookAsync(Book book) => await _bookDAL.SaveAsync(book);

        public async Task<ResponseApi<Book>> GetBookByIdAsync(string id) => new ResponseApi<Book>(await _bookDAL.GetByIdAsync(id));

        public async Task<BaseResponse> DeleteBooksAsync(ICollection<string> ids)
        {
            await _bookDAL.RemoveAsync(ids);
            return new BaseResponse("Books Delete Successfully", HttpStatusCode.OK);
        }
    }
}