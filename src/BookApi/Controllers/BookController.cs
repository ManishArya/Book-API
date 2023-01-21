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
using Microsoft.Extensions.Configuration;

namespace BookApi.Controllers
{
    [Route("api/v1/Book")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        private readonly IStringLocalizer<Localizations> _localizer;

        private readonly IConfiguration _configuration;

        public BookController(ILogger<BookController> logger, IBookService bookService, IStringLocalizer<Localizations> localizer, IConfiguration configuration) : base(logger)
        {
            _bookService = bookService;
            _localizer = localizer;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("list")]
        public async Task<IActionResult> GetBooks(BookCriteria criteria)
        {
            if (criteria == null)
            {
                return BadRequest();

            }

            var response = await _bookService.GetBooksAsync(criteria);
            return ToSendResponse(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBook(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await _bookService.GetBookByIdAsync(id);
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

                await _bookService.AddBookAsync(book);
                return ToSendResponse();
            }

            return ToSendResponse(ModelState);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteBooks(ICollection<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest();
            }

            var response = await _bookService.DeleteBooksAsync(ids);
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

            if (!CheckPosterExtensionAndFormat(poster, out errorMessage))
            {
                return false;
            }

            var allowedFileSize = 1 * 1024 * 1000;

            if (poster.Length > allowedFileSize)
            {
                errorMessage = _localizer["posterSizeValidation"].Value;
                return false;
            }
            return true;
        }

        private bool CheckPosterExtensionAndFormat(IFormFile poster, out string errorMessage)
        {
            errorMessage = string.Empty;
            var validPosterExtensions = _configuration.GetSection("ValidPosterExtensions").Get<string[]>();
            var extension = poster.FileName.GetFileExtension();
            var type = poster.ContentType;

            if (!validPosterExtensions.Any(v => v == extension) || !type.StartsWith("image", StringComparison.OrdinalIgnoreCase))
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
