using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.DataAccess
{
    public interface IBaseDAL<T>
    {
        Task<IEnumerable<T>> GetAll();

        Task<bool> Save(T baseObject);

        Task<T> GetById(string id);

        Task<bool> Remove(IEnumerable<string> ids);

        Task<bool> Remove(string id);

        Task<bool> RemoveAll();
    }
}