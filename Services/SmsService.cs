using CustomResponce.Models;
using Fetch;
using OTPService.Common;
using OTPService.DTOs;

namespace OTPService.Services
{
    public class SmsService
    {

        private readonly FetchHttpRequest _fetch;

        public SmsService(IHttpClientFactory httpClientFactory)
        {
            FetchClientOptions fetchClientOptions = new()
            {
                BaseUrl = "https://api.kavenegar.com/v1/"
            };
            _fetch = new(httpClientFactory, fetchClientOptions);
        }


        public async Task<Result> SendCode(SendCodeDto dto)
        {
            var result = new Result();
            var code = new Random().Next(1000, 9999).ToString();
            //TODO: SetOnConfig
            const string API_KEY = "51566F5254736B6161375775564279316957454E4F5436527A6E70536756454E";
            dto.SenderNumber = "10008663";
            dto.Message = "کد احراز هویت شما جهت ورود به سامانه: " + code;

            //TODO: Make Dependency Inversion
            FetchRequestOptions options = new()
            {
                Url = $@"{API_KEY}/verify/lookup.json",
                Params = $@"?receptor={dto.Mobile}&template=&token={code}&template=verify"
            };
            var responce = await _fetch.Get(options);

            if (!responce.Status)
                return CustomErrors.SendCodeFailed();

            return CustomResults.CodeSent(code);
        }
    }
}