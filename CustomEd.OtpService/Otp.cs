using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomEd.OtpService;

public class Otp
{
    public Guid Id {set; get;}
    public string EmailAddress { get; set; }
    public string OtpCode { get; set; }
    public DateTime SentAt {get; set;} = DateTime.UtcNow;
}
