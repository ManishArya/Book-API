using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using BookApi.services;
using System.Threading.Tasks;
using System;
using BookApi.constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using BookApi.models;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookController : ControllerBase
    {

        private readonly ILogger<BookController> _logger;
        private readonly IBookService _bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInformation($"{nameof(BookController)}.{nameof(GetBooks)} beginning {Request.Path}");

                var movies = await _bookService.GetBooks();

                _logger.LogInformation($"{nameof(BookController)}.{nameof(GetBooks)} returning");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBook(string id)
        {
            try
            {

                if (id == null)
                {
                    return BadRequest();
                }

                _logger.LogInformation($"{nameof(BookController)}.{nameof(GetBook)} beginning {Request.Path}{Request.QueryString}");

                var result = await _bookService.GetBookById(id);

                _logger.LogInformation($"{nameof(BookController)}.{nameof(GetBook)} returning");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] string bookString, [FromForm] IFormFile poster)
        {
            if (poster == null || bookString == null)
            {
                return BadRequest();
            }

            try
            {
                _logger.LogInformation($"{nameof(BookController)}.{nameof(AddBook)} beginning {Request.Path}");

                var book = JsonConvert.DeserializeObject<Book>(bookString);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await poster.OpenReadStream().CopyToAsync(memoryStream);
                    book.Poster = memoryStream.ToArray();
                }

                await _bookService.AddBook(book);

                _logger.LogInformation($"{nameof(BookController)}.{nameof(AddBook)} returning");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        [HttpDelete]

        public async Task<IActionResult> DeleteBook(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {

                _logger.LogInformation($"{nameof(BookController)}.{nameof(DeleteBook)} beginning {Request.Path}{Request.QueryString}");

                await _bookService.DeleteBook(id);

                _logger.LogInformation($"{nameof(BookController)}.{nameof(DeleteBook)} returning");

                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }
    }
}
