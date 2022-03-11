using System;
using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser(string userName, string name) : base(userName)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = userName;
        }

        public string Name { get; set; }
    }
}

