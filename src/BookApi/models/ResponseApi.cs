namespace BookApi.models
{
    public class ResponseApi<T>
    {
        public bool IsSuccess { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

    }
}