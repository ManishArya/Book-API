namespace BookApi.models
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }
    }
}