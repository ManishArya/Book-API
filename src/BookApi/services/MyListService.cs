using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<ResponseApi<IEnumerable<MyList>>> GetMyList() => new ResponseApi<IEnumerable<MyList>>(await _myListDAL.GetAll());

        public async Task<BaseResponse> AddItemToMyList(string bookId)
        {
            var book = await _bookService.GetBookById(bookId);

            if (book.Content == null)
            {
                return new BaseResponse("No Book found", HttpStatusCode.NotFound);
            }

            var isExists = await _myListDAL.CheckItemExistsInMyList(book.Content.Id);

            if (isExists)
            {
                throw new InvalidOperationException("This book is already exists in MyList");
            }

            await _myListDAL.AddItemToMyList(book.Content.Id);

            return new BaseResponse("Book Added in my list");
        }

        public async Task<BaseResponse> RemoveItemFromMyList(string itemId)
        {
            var isExists = await _myListDAL.CheckItemExistsInMyList(itemId);

            if (isExists)
            {
                await _myListDAL.DeleteItemFromMyList(itemId);
                return new BaseResponse("Book removed from my list successfully", HttpStatusCode.OK);
            }

            return new BaseResponse("No Book found", HttpStatusCode.Gone);
        }

        public async Task<ResponseApi<bool>> CheckItemInMyList(string itemId)
        {
            var result = await _myListDAL.CheckItemExistsInMyList(itemId);
            return new ResponseApi<bool>(result);
        }
    }
}