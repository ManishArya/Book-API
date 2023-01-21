using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IBookService
    {
        Task<PageResponseApi<IEnumerable<Book>>> GetBooksAsync(BookCriteria criteria);

        Task<ResponseApi<Book>> GetBookByIdAsync(string id);

        Task AddBookAsync(Book book);

        Task<BaseResponse> DeleteBooksAsync(ICollection<string> ids);
    }
}