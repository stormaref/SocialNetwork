using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Services
{
    public static class Extensions
    {
        public static int GetUserId(this ControllerBase controller)
        {
            return int.Parse(controller.Request.HttpContext.User.Claims.First(c => c.Type == "userId").Value);
        }

        public static long GetTimeStamp(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }
}
