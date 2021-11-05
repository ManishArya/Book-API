using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;

namespace BookApi.services
{
    public class MyListService : IMyListService
    {
        private readonly IMyListDAL _myListDAL;

        private readonly IBookService _bookService;

        public MyListService(IMyListDAL myListDAL, IBookService bookService)
        {
            _myListDAL = myListDAL;
            _bookService = bookService;
        }

        public async Task<ResponseApiList<MyList>> GetMyList() => new ResponseApiList<MyList> { Data = await _myListDAL.GetAll() };

        public async Task<ResponseApi<string>> AddItemToMyList(string bookId)
        {
            var book = await _bookService.GetBookById(bookId);

            if (book == null)
            {
                return null;
            }

            var isExists = await _myListDAL.CheckItemExistsInMyList(book.Id);

            if (isExists)
            {
                return new ResponseApi<string>() { Message = "Duplicate record" };
            }

            await _myListDAL.AddItemToMyList(book.Id);

            return new ResponseApi<string>();
        }

        public async Task<ResponseApi<string>> RemoveItemFromMyList(string itemId)
        {
            var isExists = await _myListDAL.CheckItemExistsInMyList(itemId);

            if (isExists)
            {
                await _myListDAL.DeleteItemFromMyList(itemId);
                return new ResponseApi<string>();
            }

            return null;
        }
    }
}