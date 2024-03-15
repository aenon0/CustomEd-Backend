namespace CustomEd.Shared.Settings;

public class MongoSettings
{
    public required string Host{get; init;}
    public int Port { get; init; }
    public string ConnectionString => $"mongodb://{Host}:{Port}";
}