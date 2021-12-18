using System.Net;
using System.Text.Json.Serialization;

namespace BookApi.models
{
    public class BaseResponse
    {
        public BaseResponse(string description) : this(description, HttpStatusCode.OK)
        {

        }

        public BaseResponse(string description, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : this(statusCode)
        {
            ErrorDescription = description;
        }

        protected BaseResponse(HttpStatusCode statusCode)
        {
            StatusCode = (int)statusCode;
        }

        public bool IsSuccess { get => StatusCode == (int)HttpStatusCode.OK; }


        [JsonIgnore]
        public int StatusCode { get; set; }

        public string ErrorDescription { get; set; }

    }
}