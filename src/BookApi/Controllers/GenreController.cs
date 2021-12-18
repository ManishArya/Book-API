using System.Threading.Tasks;
using BookApi.filters;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookApi.Controllers
{
    [Route("api/Genre")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]

    public class GenreController : BaseController
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService, ILogger<GenreController> logger) : base(logger)
        {
            _genreService = genreService;
        }

        #region Genres

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            var value = await _genreService.GetGenres();
            return ToSendResponse(value);
        }

        #endregion Genres
    }
}