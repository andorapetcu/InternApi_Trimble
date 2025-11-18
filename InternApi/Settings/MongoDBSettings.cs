namespace InternApi.Settings
{
    /// <summary>
    /// Implementation of the interface that holds information about the Mongo database connection that are stored in appsettings.Development.json
    /// </summary>
    public class MongoDBSettings : IMongoDBSettings
    {
        public string NoteCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
