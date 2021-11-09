using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using BookApi.services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using BookApi.models;
using BookApi.filters;
namespace BookApi.Controllers
{
    [Route("api/Book")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService) : base(logger)
        {
            _bookService = bookService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBooks()
        {
            var movies = await _bookService.GetBooks();
            return Ok(movies);
        }

        [HttpGet]
        public async Task<IActionResult> GetBook(string id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            var result = await _bookService.GetBookById(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] string bookString, [FromForm] IFormFile poster)
        {
            if (bookString == null)
            {
                return BadRequest();
            }

            if (poster == null)
            {
                ModelState.AddModelError("Poster", "The Poster field is required");
            }

            var book = JsonConvert.DeserializeObject<Book>(bookString);

            if (TryValidateModel(book))
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await poster.OpenReadStream().CopyToAsync(memoryStream);
                    book.Poster = memoryStream.ToArray();
                }

                await _bookService.AddBook(book);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete]

        public async Task<IActionResult> DeleteBook(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            await _bookService.DeleteBook(id);

            return NoContent();
        }
    }
}
