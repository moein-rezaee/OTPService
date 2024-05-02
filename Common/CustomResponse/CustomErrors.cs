using CustomResponce.Models;

namespace OTPService.Common
{
    public class CustomErrors
    {
        public static Result InvalidMobileNumber() => new()
        {
            Message = new()
            {
                Fa = "شماره همراه وارد شده معتبر نمی باشد",
                En = "Invalid Mobile Number"
            },
            StatusCode = StatusCodes.Status400BadRequest,
            Status = false
        };


        public static Result HttpRequestFailed(HttpResponseMessage data) => new ()
        {
            Message = new Message()
            {
                Fa = "خطا هنگام ارسال درخواست",
                En = "Send Http Request Failed!"
            },
            Data = data,
            StatusCode = (int)data.StatusCode,
            Status = false
        };

        public static Result SendCodeFailed() => new ()
        {
            Message = new Message()
            {
                Fa = "خطا هنگام ارسال کد",
                En = "BadGateway. Send Code Failed!"
            },
            StatusCode = StatusCodes.Status502BadGateway,
            Status = false
        };

        public static Result SendCodeServerError() => new ()
        {
            Message = new Message()
            {
                Fa = "خطای سرور هنگام ارسال کد",
                En = "InternalServerError. Sending Code Failed!"
            },
            StatusCode = StatusCodes.Status500InternalServerError,
            Status = false
        };

        public static Result VerifyCodeServerError() => new ()
        {
            Message = new Message()
            {
                Fa = "خطای سرور هنگام تایید کد",
                En = "InternalServerError. Verify Code Failed!"
            },
            StatusCode = StatusCodes.Status500InternalServerError,
            Status = false
        };
    }
}
