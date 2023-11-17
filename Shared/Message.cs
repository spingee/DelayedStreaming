namespace Shared;

public static class Constants
{
    public const string StreamName = nameof(StreamName);
    public const string SMSProvider = nameof(SMSProvider);
}

[GenerateSerializer]
public record Message(DateTime Timestamp);