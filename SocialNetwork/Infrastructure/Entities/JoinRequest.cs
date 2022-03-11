using SocialNetwork.Services;

namespace SocialNetwork.Infrastructure.Entities
{
    public class JoinRequest : BaseEntity
    {
        public JoinRequest()
        {

        }

        public JoinRequest(int groupId, int userId)
        {
            GroupId = groupId;
            UserId = userId;
        }

        public int Id { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
        public long Date { get; set; } = DateTime.Now.GetTimeStamp();
    }
}
