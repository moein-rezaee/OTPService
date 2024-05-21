using CustomResponce.Models;
using Kavenegar.Core.Exceptions;
using OTPService.Common;
using OTPService.DTOs;

namespace OTPService.Services
{
    public class SmsService
    {
        public async Task<Result> SendSms(SendSmsDto dto)
        {
            const string API_KEY = "51566F5254736B6161375775564279316957454E4F5436527A6E70536756454E";
            const string SENDER = "10008663";
            try
            {
                Kavenegar.KavenegarApi api = new(API_KEY);
                var result = await api.Send(SENDER, dto.Receivers, dto.Message);
                return CustomResults.CodeSent(result);
            }
            catch (ApiException ex)
            {
                return CustomErrors.SendSmsFailed();
            }
            catch (HttpException ex)
            {
                return CustomErrors.InvalidSendSms();
            }
        }
    }
}