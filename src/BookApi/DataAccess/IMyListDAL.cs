using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.DataAccess
{
    public interface IMyListDAL : IBaseDAL<MyList>
    {
        Task<bool> AddItemToMyList(MyList myList);

        Task<bool> CheckItemExistsInMyList(string itemId);

        Task<bool> DeleteItemFromMyList(string itemId);

        Task<int> GetListCounts();
    }
}