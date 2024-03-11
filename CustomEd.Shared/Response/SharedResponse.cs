namespace CustomEd.Shared.Response;

public class SharedResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; } = null!;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static SharedResponse<T> Success(T? data, string? message)
    {
        return new SharedResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }
    public static SharedResponse<T> Fail(string? message, List<string>? errors)
    {
        return new SharedResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }

}
