using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Fetch
{
    public class FetchRequestOptions : FetchClientOptions
    {
        public required string Url { get; set; }
        public object? Data { get; set; }
        public string Params { get; set; } = "";
        public string MediaType { get; set; } = Application.Json;

        public string FullUrl { get => BaseUrl + Url + Params; }
        public string DataToJson { get => JsonSerializer.Serialize(Data); }
        public StringContent Content
        {
            get => new(DataToJson, Encoding.UTF8, MediaType);
        }
    }
}
