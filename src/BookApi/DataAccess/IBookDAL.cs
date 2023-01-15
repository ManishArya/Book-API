using System.Threading.Tasks;
using System.Collections.Generic;
using BookApi.models;

namespace BookApi.DataAccess
{
    public interface IBookDAL : IBaseDAL<Book>
    {
        Task<bool> RemoveAsync(IEnumerable<string> ids);
    }
}