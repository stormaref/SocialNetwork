using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser(string userName, string name) : base(userName)
        {
            Name = name;
            Email = userName;
        }

        public string Name { get; set; }
        public Group? Group { get; set; }
        public int? GroupId { get; set; }
        public DateTime? JoinDate { get; set; }
        public ICollection<JoinRequest> JoinRequests { get; set; }
    }
}

