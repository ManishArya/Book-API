using System.Collections.Generic;

namespace BookApi.models
{
    public class ResponseApiList<T> : BaseResponse
    {
        public ResponseApiList() : base("")
        {

        }
        public IEnumerable<T> Content { get; set; }
    }
}