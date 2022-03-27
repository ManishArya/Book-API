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
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.Extensions.Localization;

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

        private readonly IStringLocalizer<localizations> _localizer;

        public BookController(ILogger<BookController> logger, IBookService bookService, IMyListService myListService, IStringLocalizer<localizations> localizer) : base(logger)
        {
            _bookService = bookService;
            _myListService = myListService;
            this._localizer = localizer;
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
                errorMessage = _localizer["posterRequired"].Value;
                return false;
            }

            if (poster.Length == 0)
            {
                errorMessage = _localizer["posterContentLengthvalidation"].Value;
                return false;
            }

            var allowedFileSize = 1 * 1024 * 1000;

            if (poster.Length > allowedFileSize)
            {
                errorMessage = _localizer["posterSizeValidation"].Value;
                return false;
            }

            if (!IsValidImage(poster, out errorMessage))
            {
                return false;
            }
            return true;
        }

        private bool IsValidImage(IFormFile poster, out string errorMessage)
        {
            errorMessage = string.Empty;

            var extension = poster.FileName.ToGetExtension();
            var type = poster.ContentType;

            if (!validExtensions.Any(v => v == extension) || !type.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = _localizer["posterExtensionsValidation"].Value;
                return false;
            }

            var inspector = new FileFormatInspector();
            var format = inspector.DetermineFileFormat(poster.OpenReadStream());

            if (!(format is Image))
            {
                errorMessage = _localizer["posterContentValidation"].Value;
                return false;
            }

            return true;
        }
    }
}
