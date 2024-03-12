namespace CustomEd.Shared.Settings;

public class JWTSettings
{
    public string Key { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public TimeSpan TokenLifetime { get; init; }
}