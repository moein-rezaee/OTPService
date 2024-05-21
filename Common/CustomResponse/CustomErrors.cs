using CustomResponce.Models;

namespace OTPService.Common
{
    public class CustomErrors
    {
        public static Result InvalidInputData(object? errors) => new()
        {
            Message = new()
            {
                Fa = "داده های ورودی نا معتبر می باشد",
                En = "Invalid Input Data" 
            },
            Data = errors,
            StatusCode = StatusCodes.Status400BadRequest,
            Status = false
        };

        public static Result InvalidSendSms(object? data = null) => new()
        {
            Message = new Message()
            {
                Fa = "ارسال پیام مجاز نمی باشد",
                En = "Send SMS Is Invalid!"
            },
            Data = data,
            StatusCode = StatusCodes.Status401Unauthorized,
            Status = false
        };

        public static Result SendSmsFailed(object? data = null) => new()
        {
            Message = new Message()
            {
                Fa = "خطای برقراری ارتباط با سرویس دهند. لطفا دقایقی دیگر مجددا تلاش نمایید.",
                En = "Bad Gateway. Send SMS Failed!"
            },
            Data = data,
            StatusCode = StatusCodes.Status502BadGateway,
            Status = false
        };

        public static Result SendSmsServerError(object? data) => new()
        {
            Message = new Message()
            {
                Fa = "خطای سرور هنگام ارسال پیام",
                En = "InternalServerError. Sending SMS Failed!"
            },
            Data = data,
            StatusCode = StatusCodes.Status500InternalServerError,
            Status = false
        };
    }
}
