using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.DataAccess
{
    public interface IBaseDAL<T>
    {
        Task<IEnumerable<T>> GetAll();

        Task<bool> Save(T baseObject);

        Task<T> GetById(string id);

        Task<bool> Remove(List<string> ids);
    }
}