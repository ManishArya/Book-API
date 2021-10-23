using System.Collections.Generic;

namespace BookApi.models
{
    public class ResponseApiList<T>
    {
        public IEnumerable<T> Data { get; set; }
    }
}