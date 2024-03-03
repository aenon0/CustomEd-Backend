using MongoDB.Driver;
namespace CustomEd.OtpService.Database;
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    private readonly IMongoDatabase database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public IMongoCollection<Otp> GetCollection<Otp>(string collectionName)
    {
        return _database.GetCollection<Otp>(collectionName);
    }
}
