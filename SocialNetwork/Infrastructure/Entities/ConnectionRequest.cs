using SocialNetwork.Services;

namespace SocialNetwork.Infrastructure.Entities
{
    public class ConnectionRequest : BaseEntity
    {
        public ConnectionRequest(int senderGroupId, int receiverGroupId)
        {
            SenderGroupId = senderGroupId;
            ReceiverGroupId = receiverGroupId;
        }

        public int Id { get; set; }
        public int SenderGroupId { get; set; }
        public int ReceiverGroupId { get; set; }
        public long Sent { get; set; } = DateTime.Now.GetTimeStamp();
    }
}
