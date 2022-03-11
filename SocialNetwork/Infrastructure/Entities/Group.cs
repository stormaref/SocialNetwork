namespace SocialNetwork.Infrastructure.Entities
{
    public class Group : BaseEntity
    {
        public Group()
        {
        }

        public Group(ApplicationUser creator, string name, string description)
        {
            AdminId = creator.Id;
            Members = new List<ApplicationUser>();
            Members.Add(creator);
            Name = name;
            Description = description;
            creator.JoinDate = DateTime.Now;
            ConnectedGroups = new List<Group>();
        }

        public int Id { get; set; }
        public ICollection<ApplicationUser> Members { get; set; }
        public int AdminId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<JoinRequest> JoinRequests { get; set; }
        public ICollection<Group> ConnectedGroups { get; set; }
    }
}

