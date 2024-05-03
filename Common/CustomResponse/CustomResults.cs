
using CustomResponce.Models;

namespace OTPService.Common
{
    public class CustomResults
    {
        public static Result UserCreated(object data) => new()
        {
            Message = new()
            {
                Fa = "عملیات ایجاد کاربر با موفقیت انجام شد",
                En = "Create User Done"
            },
            StatusCode = StatusCodes.Status201Created,
            Data = data
        };

        public static Result HttpRequestOk(HttpResponseMessage data) => new()
        {
            Message = new Message()
            {
                Fa = "عملیات با موفقیت به پایان رسید",
                En = "Good Job. Result is Ok!"
            },
            Data = data,
            StatusCode = (int)data.StatusCode,
        };

        public static Result CodeSent(object data) => new()
        {
            Message = new Message()
            {
                Fa = "ارسال کد یکبارمصرف با موفقیت انجام شد",
                En = "Good Job. Code Sent!"
            },
            Data = data,
            StatusCode = StatusCodes.Status200OK,
        };
        
        public static Result ValidCode() => new()
        {
            Message = new Message()
            {
                Fa = "کد وارد شده معتبر می باشد",
                En = "Good Job. Code is Valid!"
            },
            StatusCode = StatusCodes.Status202Accepted,
        };
    }
}