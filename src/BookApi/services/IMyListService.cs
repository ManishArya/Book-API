using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IMyListService
    {
        Task<ResponseApi<string>> AddItemToMyList(string ItemId);

        Task<ResponseApi<string>> RemoveItemFromMyList(string itemId);

        Task<ResponseApiList<MyList>> GetMyList();

        Task<bool> CheckItemInMyList(string id);
    }
}