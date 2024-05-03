using System.Text.Json;
using CustomResponce.Models;
using OTPService.Common;

namespace Fetch
{
    public class FetchHttpRequest
    {
        private readonly HttpClient _httpClient;
        private readonly FetchClientOptions _options;

        public FetchHttpRequest(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }


        public FetchHttpRequest(IHttpClientFactory httpClientFactory, FetchClientOptions options)
        {
            AddHeaders(options.Headers);
            _httpClient = httpClientFactory.CreateClient();
            _options = options;
        }


        public async Task<Result> Get(FetchRequestOptions options)
        {
            options.BaseUrl = string.IsNullOrEmpty(options.BaseUrl) ?  _options.BaseUrl : options.BaseUrl; 
            AddHeaders(options.Headers);
            var response = await _httpClient.GetAsync(options.FullUrl);
            return GetRequestResult(response);
        }

        public async Task<Result> Post(FetchRequestOptions options)
        {   
            options.BaseUrl = string.IsNullOrEmpty(options.BaseUrl) ?  _options.BaseUrl : options.BaseUrl; 
            AddHeaders(options.Headers);
            using var response = await _httpClient.PostAsync(options.FullUrl, options.Content);
            return GetRequestResult(response);
        }

        public async Task<T?> GetData<T>(HttpResponseMessage response)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                T? data = await JsonSerializer.DeserializeAsync<T>(contentStream);
                return data;
            }
            return default;
        }

        private void AddHeaders(List<FetchHttpHeader>? headers)
        {
            if (headers != null)
                foreach (var header in headers)
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        private Result GetRequestResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return CustomResults.HttpRequestOk(response);
            return CustomErrors.HttpRequestFailed(response);
        }

    }
}