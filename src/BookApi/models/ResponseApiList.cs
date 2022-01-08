using System.Collections.Generic;

namespace BookApi.models
{
    public class ResponseApiList<T> : BaseResponse
    {
        public ResponseApiList(IEnumerable<T> content) : base(System.Net.HttpStatusCode.OK) => Content = content;

        public IEnumerable<T> Content { get; set; }
    }
}