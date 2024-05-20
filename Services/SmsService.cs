using CustomResponce.Models;
using Fetch;
using Kavenegar.Core.Exceptions;
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
            // var result = new Result();
            // var code = new Random().Next(1000, 9999).ToString();
            // //TODO: SetOnConfig
            const string API_KEY = "51566F5254736B6161375775564279316957454E4F5436527A6E70536756454E";
            // dto.SenderNumber = "10008663";
            // dto.Message = "کد احراز هویت شما جهت ورود به سامانه: " + code;

            // //TODO: Make Dependency Inversion
            // FetchRequestOptions options = new()
            // {
            //     Url = $@"{API_KEY}/sms/send.json",
            //     Params = $@"?receptor={dto.Mobile}&sender={dto.SenderNumber}&message={dto.Message}"
            // };
            // var responce = await _fetch.Get(options);

            // if (!responce.Status)
            //     return CustomErrors.SendCodeFailed();

            // return CustomResults.CodeSent(code);

            try
            {
                Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(API_KEY);
                var result = await api.Send("10008663", "09308021771", "خدمات پیام کوتاه کاوه نگار");
                return CustomResults.CodeSent(result);
            }
            catch (ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
                return CustomErrors.SendCodeFailed();
            }
            catch (HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
                return CustomErrors.SendCodeFailed();
            }
        }
    }
}