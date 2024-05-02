namespace CustomResponce.Models
{
    public class Result
    {
        public Message Message { get; set; } = new();
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public object? Data { get; set; }
        public bool Status { get; set; } = true;
    }
}