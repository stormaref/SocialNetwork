namespace SocialNetwork.Infrastructure.Entities
{
    public class Chat : BaseEntity
    {
        public Chat()
        {

        }

        public Chat(int firstUserId, int secondUserId, Message message)
        {
            FirstUserId = firstUserId;
            SecondUserId = secondUserId;
            Messages = new List<Message>();
            Messages.Add(message);
        }

        public int Id { get; set; }
        public int FirstUserId { get; set; }
        public int SecondUserId { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
