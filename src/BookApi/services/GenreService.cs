using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;

namespace BookApi.services
{
    public class GenreService : IGenreService
    {
        private readonly IBaseDAL<Genre> _genreDAL;

        public GenreService(IBaseDAL<Genre> genreDAL) => _genreDAL = genreDAL;

        public async Task<ResponseApi<IEnumerable<Genre>>> GetGenres() => new ResponseApi<IEnumerable<Genre>>(await _genreDAL.GetAll());
    }
}