namespace SocialNetwork.Infrastructure.Entities
{
    public abstract class BaseEntity
    {
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
