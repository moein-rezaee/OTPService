using CustomResponse.Models;
using Fetch;
using OTPService.Common;
using OTPService.DTOs;

namespace OTPService.Services
{
    public class SmsService
    {

        private readonly IConfiguration _config;
        private readonly FetchHttpRequest _fetch;

        public SmsService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _config = config;
            FetchClientOptions fetchClientOptions = new()
            {
                BaseUrl = "https://api.kavenegar.com/v1/"
            };
            _fetch = new(httpClientFactory, fetchClientOptions);
        }


        public async Task<Result> SendCode(SendCodeDto dto)
        {
            Result result = new();
            string code = new Random().Next(1000, 9999).ToString();
            string? API_KEY = _config.GetSection("SMSProvider:API_KEY").Value;

            //TODO: Make Dependency Inversion
            //TODO: Use SDK
            FetchRequestOptions options = new()
            {
                Url = $@"{API_KEY}/verify/lookup.json",
                Params = $@"?receptor={dto.Mobile}&token={code}&template=verify"
            };
            Result response = await _fetch.Get(options);

            // FIXME: Uncomment
            if (!response.Status)
                return CustomErrors.SendCodeFailed();

            return CustomResults.CodeSent(code);
        }
    }
}