using Microsoft.AspNetCore.Http;

namespace BookApi.models
{
    public class BookForm
    {
        public string Content { get; set; }

        public IFormFile Poster { get; set; }
    }
}