namespace Shared.SmsProvider.Core
{
    public class SmsResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private SmsResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static SmsResult Ok(string messageId) => new(true, messageId);
        public static SmsResult Fail(string error) => new(false, error);
    }
}