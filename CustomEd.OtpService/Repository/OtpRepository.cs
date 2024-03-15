using MongoDB.Driver;

namespace CustomEd.OtpService.Repository;

public class OtpRepository : IOtpRepository
{
    private readonly IMongoCollection<Otp> _collection;

    public OtpRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Otp>(typeof(Otp).Name);
    }

    public async Task Add(Otp otp)
    {
        await _collection.InsertOneAsync(otp);
    }

    public async Task<bool> ExistsByEmailAddress(string emailAddress)
    {
        var filter = Builders<Otp>.Filter.Eq(x => x.EmailAddress, emailAddress);
        var count = await _collection.CountDocumentsAsync(filter);
        return count > 0;
    }

    public async Task<Otp> GetByEmailAddress(string emailAddress)
    {
        return await _collection.Find(x => x.EmailAddress == emailAddress).FirstOrDefaultAsync();
    }

    public async Task Update(Otp otp)
    {
        await _collection.ReplaceOneAsync(x => x.Id == otp.Id, otp);
    }
}
