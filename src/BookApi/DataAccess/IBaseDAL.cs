using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.DataAccess
{
    public interface IBaseDAL<T>
    {
        Task<IEnumerable<T>> GetAsync();

        Task SaveAsync(T baseObject);

        Task<T> GetByIdAsync(string id);

        Task<bool> RemoveAllAsync();
    }
}