using SocialNetwork.Services;

namespace SocialNetwork.Infrastructure.Entities
{
    public class Message : BaseEntity
    {
        public Message()
        {

        }

        public Message(int senderId, int receiverId, string body)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Body = body;
        }

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Body { get; set; }
        public Chat Chat { get; set; }
        public int ChatId { get; set; }
        public long Date { get; set; } = DateTime.Now.GetTimeStamp();
    }
}