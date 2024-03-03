using MongoDB.Bson;
using MongoDB.Driver;


namespace ISchool.Repositories;

public class GenericRepository<T> 
{
    private readonly IMongoCollection<T> _collection;

    public GenericRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name);
    }

    public async Task<bool> Exists(string email)
    {
        var filter = Builders<T>.Filter.Eq("Email", email);
        var count = await _collection.CountDocumentsAsync(filter);
        return count > 0;
    }
}

