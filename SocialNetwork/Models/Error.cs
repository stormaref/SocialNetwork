namespace SocialNetwork.Models
{
    public class Error
    {

        public string EnMessage { get; set; }

        public Error(string enMessage)
        {
            EnMessage = enMessage;
        }
    }
}
