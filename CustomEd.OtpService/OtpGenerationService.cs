using System;
using System.Linq;
namespace CustomEd.OtpService;
public class OtpGenerationService
{
    private readonly Random _random;
    public OtpGenerationService()
    {
        _random = new Random();
    }


    public string GenerateOTP()
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}