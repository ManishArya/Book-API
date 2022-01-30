using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;

namespace BookApi.services
{
    public class BookService : IBookService
    {
        private readonly IBaseDAL<Book> _bookDAL;

        public BookService(IBaseDAL<Book> bookDAL) => _bookDAL = bookDAL;

        public async Task<ResponseApi<IEnumerable<Book>>> GetBooks() => new ResponseApi<IEnumerable<Book>>(await _bookDAL.GetAll());

        public async Task<BaseResponse> AddBook(Book book) { await _bookDAL.Save(book); return new BaseResponse("Book save successfully"); }

        public async Task<ResponseApi<Book>> GetBookById(string id) => new ResponseApi<Book>(await _bookDAL.GetById(id));

        public async Task<BaseResponse> DeleteBooks(List<string> ids)
        {
            await _bookDAL.Remove(ids);
            return new BaseResponse("Books Delete Successfully", HttpStatusCode.OK);
        }
    }
}