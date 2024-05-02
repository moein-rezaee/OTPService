namespace Fetch
{
    public class FetchClientOptions
    {
        public string BaseUrl { get; set; } = "";
        public List<FetchHttpHeader>? Headers { get; set; }
    }
}
