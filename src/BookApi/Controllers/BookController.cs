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
using System.Linq;
using BookApi.extensions;
using System;

namespace BookApi.Controllers
{
    [Route("api/Book")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]
    public class BookController : BaseController
    {
        private readonly string[] validExtensions = new string[] { "png", "jpg", "jpeg", "gif" };
        private readonly IBookService _bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService) : base(logger)
        {
            _bookService = bookService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBooks()
        {
            var response = await _bookService.GetBooks();
            return ToSendResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetBook(string id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            var result = await _bookService.GetBookById(id);

            return ToSendResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] string bookString, [FromForm] IFormFile poster)
        {
            if (bookString == null)
            {
                return BadRequest();
            }

            if (!this.IsPosterIsImage(poster, out string errorMessage))
            {

                ModelState.AddModelError("Poster", errorMessage);
            }

            var book = JsonConvert.DeserializeObject<Book>(bookString);

            if (TryValidateModel(book))
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await poster.OpenReadStream().CopyToAsync(memoryStream);
                    book.Poster = memoryStream.ToArray();
                }

                var response = await _bookService.AddBook(book);

                return ToSendResponse(response);
            }

            return ToSendResponse(ModelState);
        }

        [HttpDelete]

        public async Task<IActionResult> DeleteBook(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var response = await _bookService.DeleteBook(id);

            return ToSendResponse(response);
        }

        private bool IsPosterIsImage(IFormFile poster, out string errorMessage)
        {
            if (poster == null)
            {
                errorMessage = "Poster field is required";
                return false;
            }

            var extension = poster.FileName.ToGetExtension();

            if (!validExtensions.Any(v => v == extension))
            {
                errorMessage = "Poster must be either of jpg or png or gif.";
                return false;
            }

            var type = poster.ContentType;

            if (!type.Contains("image/", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Poster must be either of jpg or png or gif.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
