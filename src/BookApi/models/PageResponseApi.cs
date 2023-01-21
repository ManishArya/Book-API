using System.Net;

namespace BookApi.models
{
    public class PageResponseApi<T> : ResponseApi<T>
    {
        public PageResponseApi(T content, long count) : base(content) => Count = count;

        public long Count { get; set; }
    }
}