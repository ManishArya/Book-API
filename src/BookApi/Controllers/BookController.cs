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
using System.Collections.Generic;

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

        private readonly IMyListService _myListService;

        public BookController(ILogger<BookController> logger, IBookService bookService, IMyListService myListService) : base(logger)
        {
            _bookService = bookService;
            _myListService = myListService;
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
        public async Task<IActionResult> AddBook([FromForm] BookForm bookForm)
        {
            if (bookForm.Content == null)
            {
                return BadRequest();
            }

            if (TryValidateBookForm(bookForm, out Book book))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await bookForm.Poster.OpenReadStream().CopyToAsync(memoryStream);
                    book.Poster = memoryStream.ToArray();
                }

                var response = await _bookService.AddBook(book);

                return ToSendResponse(response);
            }

            return ToSendResponse(ModelState);
        }

        [HttpPost("delete")]

        public async Task<IActionResult> DeleteBooks(List<string> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var response = await _bookService.DeleteBooks(ids);

            await _myListService.RemoveBookIdsFromMyList(ids);

            return ToSendResponse(response);
        }

        private bool TryValidateBookForm(BookForm bookForm, out Book book)
        {
            if (!TryValidatePoster(bookForm.Poster, out string errorMessage))
            {

                ModelState.AddModelError("Poster", errorMessage);
            }

            book = JsonConvert.DeserializeObject<Book>(bookForm.Content);

            return TryValidateModel(book);
        }

        private bool TryValidatePoster(IFormFile poster, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (poster == null)
            {
                errorMessage = "poster is required.";
                return false;
            }

            if (poster.Length == 0)
            {
                errorMessage = "poster must have content.";
                return false;
            }

            if (!IsPosterValidImage(poster))
            {
                errorMessage = "uploaded poster can either jpg or png or gif.";
                return false;
            }

            var allowedFileSize = 1 * 1024 * 1000;

            if (poster.Length > allowedFileSize)
            {
                errorMessage = "poster size cannot  more than 1 MB.";
                return false;
            }

            return true;
        }

        private bool IsPosterValidImage(IFormFile poster)
        {
            var extension = poster.FileName.ToGetExtension();

            if (!validExtensions.Any(v => v == extension))
            {
                return false;
            }

            var type = poster.ContentType;

            if (!type.Contains("image/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
