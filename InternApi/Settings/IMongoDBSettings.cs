namespace InternApi.Settings
{ 
    /// <summary>
    /// Interface for holding settings regarding the Mongo database connection. 
    /// </summary>
    public interface IMongoDBSettings
    {
        string NoteCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
