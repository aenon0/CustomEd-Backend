using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomEd.OtpService;

public class Otp
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string EmailAddress { get; set; }
    public string OtpCode { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
