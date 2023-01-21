using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookApi.DataAccess
{
    public interface IBaseDAL<T>
    {
        Task<IEnumerable<T>> GetDocumentsAsync();

        Task SaveDocumentAsync(T baseObject);

        Task<T> GetDocumentByIdAsync(string id);

        Task<bool> DeleteDocumentsAsync();
    }
}