using System;
using System.Linq;
namespace CustomEd.User.Service;
public static class OtpGenerationService
{
    private static  Random _random = new Random();
    

    public static string GenerateOTP()
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}