namespace SmsExtension.Core;

public class SmsResult
{
    public bool Success { get; private set; }
    public string? MessageId { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static SmsResult Ok(string? messageId = null)
        => new SmsResult { Success = true, MessageId = messageId };

    public static SmsResult Fail(string? errorMessage = null, string? errorCode = null)
        => new SmsResult { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
}
