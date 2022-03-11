using System;
namespace SocialNetwork.Infrastructure.Entities
{
	public class Group
	{
		public Group()
		{
		}

        public ICollection<ApplicationUser> Users { get; set; }
        public ApplicationUser Admin { get; set; }
    }
}

