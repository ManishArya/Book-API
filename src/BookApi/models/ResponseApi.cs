namespace BookApi.models
{
    public class ResponseApi<T>
    {
        public string Message { get; set; }

        public T Data { get; set; }

    }
}