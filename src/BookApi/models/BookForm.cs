using Microsoft.AspNetCore.Http;

namespace BookApi.models
{
    public class BookForm
    {
        public string BookString { get; set; }

        public IFormFile Poster { get; set; }
    }
}