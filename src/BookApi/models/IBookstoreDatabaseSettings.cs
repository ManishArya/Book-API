namespace BookApi.models
{
    public interface IBookstoreDatabaseSettings
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}