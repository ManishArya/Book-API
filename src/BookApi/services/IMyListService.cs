using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IMyListService
    {
        Task<BaseResponse> AddItemToMyList(string ItemId);

        Task<BaseResponse> RemoveItemFromMyList(string itemId);

        Task<ResponseApi<IEnumerable<MyList>>> GetMyList();

        Task<ResponseApi<bool>> CheckItemInMyList(string id);
    }
}