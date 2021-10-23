namespace BookApi.models
{
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}