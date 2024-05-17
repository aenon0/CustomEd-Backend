using System.Linq.Expressions;
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

    // public async Task<bool> Exists(string email)
    // {
    //     var filter = Builders<T>.Filter.Eq("Email", email);
    //     var count = await _collection.CountDocumentsAsync(filter);
    //     return count > 0;
    // }
    public async Task<T> Get(Expression<Func<T, bool>> filter)
    {
        var result = await _collection.Find(filter).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<T>> GetAll(Expression<Func<T, bool>> filter)
    {
        var result = await _collection.Find(filter).ToListAsync();
        return result;
    }
}

