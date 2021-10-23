using System;
using System.Threading.Tasks;
using BookApi.constants;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenreService genreService, ILogger<GenreController> logger)
        {
            _genreService = genreService;
            _logger = logger;
        }

        #region Genres

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            try
            {
                _logger.LogInformation($"{nameof(GenreController.GetGenres)} begining");

                var value = await _genreService.GetGenres();

                _logger.LogInformation($"{nameof(GenreController.GetGenres)} ends");

                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        #endregion Genres
    }
}