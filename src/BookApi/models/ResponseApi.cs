using System.Net;

namespace BookApi.models
{
    public class ResponseApi<T> : BaseResponse
    {
        public ResponseApi(T content) : this(content, HttpStatusCode.OK)
        { }

        public ResponseApi(T content, HttpStatusCode statusCode) : base(statusCode) => Content = content;

        public T Content { get; set; }
    }
}