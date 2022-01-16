using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IBookService
    {
        Task<ResponseApi<IEnumerable<Book>>> GetBooks();

        Task<ResponseApi<Book>> GetBookById(string id);

        Task<BaseResponse> AddBook(Book book);

        Task<BaseResponse> DeleteBooks(List<string> ids);
    }
}