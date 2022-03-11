using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.Entities;

namespace SocialNetwork.Infrastructure.Persistence
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(a => a.GroupId);

            builder.Entity<JoinRequest>()
                .HasOne(jr => jr.Group)
                .WithMany(g => g.JoinRequests)
                .HasForeignKey(jr => jr.GroupId);

            builder.Entity<JoinRequest>()
                .HasOne(jr => jr.User)
                .WithMany(u => u.JoinRequests)
                .HasForeignKey(jr => jr.UserId);

            builder.Entity<Chat>()
                .HasAlternateKey(c => new { c.FirstUserId, c.SecondUserId });

            builder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId);

            base.OnModelCreating(builder);
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }
        public DbSet<ConnectionRequest> ConnectionRequests { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}

