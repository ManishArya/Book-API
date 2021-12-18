using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;

namespace BookApi.services
{
    public interface IGenreService
    {
        Task<ResponseApi<IEnumerable<Genre>>> GetGenres();
    }
}