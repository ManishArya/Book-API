using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.DataAccess
{
    public interface IBaseDAL<T>
    {
        Task<IEnumerable<T>> GetAll();

        Task<string> Save(T baseObject);

        Task<T> GetById(string id);

        Task<string> Remove(string id);
    }
}