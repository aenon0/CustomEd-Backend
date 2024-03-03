using MongoDB.Driver;
namespace ISchool;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    private readonly IMongoDatabase database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
