namespace SocialNetwork.Models
{
    public class ErrorResponse
    {
        public ErrorResponse(string error)
        {
            Error = new Error(error);
        }
        public Error Error { get; set; }
    }
}
