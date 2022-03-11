namespace SocialNetwork.Models
{
    public class MessageModel
    {
        public MessageModel(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
