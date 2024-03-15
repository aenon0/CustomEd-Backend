using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomEd.OtpService;

public class Otp
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id {set; get;}
    public string EmailAddress { get; set; }
    public string OtpCode { get; set; }
    public DateTime SentAt {get; set;} = DateTime.UtcNow;
}
