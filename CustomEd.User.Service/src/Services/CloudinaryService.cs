using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace CustomEd.User.Service.Services;
public class CloudinaryService 
{
    private readonly Cloudinary _cloudinary;
    private readonly string? _cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    private readonly string? _apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");


    public CloudinaryService()
    {
        if (string.IsNullOrEmpty(_cloudName) || string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
        {
            throw new InvalidOperationException("Cloudinary credentials are not set.");
        }
        var account = new Account(_cloudName, _apiKey, _apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string?> UploadImage(IFormFile image)
    {
        using var stream = new MemoryStream();
        await image.CopyToAsync(stream); // Copy the contents of the image to the memory stream
        stream.Position = 0; // Reset the position of the stream to the beginning
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription("image", stream) // Use the memory stream as the file source
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult != null && uploadResult.SecureUrl != null)
        {
            string imageUrl = uploadResult.SecureUrl.ToString();
            // Use imageUrl
            return imageUrl;    
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> DeleteImage(string imageUrl)
    {
     var uri=new Uri(imageUrl);   
     var segments=uri.Segments;
     var imageName=segments.Last().Split('.').First();

     var deletionParams=new DeletionParams(imageName);
     var result=await _cloudinary.DeleteResourcesAsync(imageName);
     return result.StatusCode == System.Net.HttpStatusCode.OK;

    }
}