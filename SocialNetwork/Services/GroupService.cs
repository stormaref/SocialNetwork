using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.Entities;
using SocialNetwork.Infrastructure.Persistence;

namespace SocialNetwork.Services
{
    public interface IGroupService
    {
        Task<GetGroupsResponse> All();
        Task<bool> CheckUserGroup(int userId);
        Task<Group> CreateGroup(ApplicationUser creator, string name, string description);
        Task<UserGroupDto?> GetUserGroup(int userId);
        Task<bool> IsConnected(Group group, int groupId);
    }
    public class GroupService : IGroupService
    {
        private readonly DatabaseContext _context;

        public GroupService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> IsConnected(Group group, int groupId)
        {
            await _context.Entry(group)
                .Collection(g => g.ConnectedGroups)
                .LoadAsync();

            return group.ConnectedGroups.Any(g => g.Id == groupId);
        }

        public async Task<UserGroupDto?> GetUserGroup(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (!user.GroupId.HasValue)
            {
                return null;
            }

            var group = await _context.Groups
                .Include(g => g.Members)
                .SingleOrDefaultAsync(g => g.Id == user.GroupId.Value);

            var members = group.Members
                .OrderBy(m => m.JoinDate)
                .Select(m => new MemberDto(m.Id, m.Name, m.Email, m.Id == group.AdminId ? "Owner" : "User"))
                .ToList();

            return new UserGroupDto(group.Name, group.Description, members);
        }

        public async Task<bool> CheckUserGroup(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user.GroupId.HasValue;
        }

        public async Task<Group> CreateGroup(ApplicationUser creator, string name, string description)
        {
            var group = new Group(creator, name, description);
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<GetGroupsResponse> All()
        {
            var groups = await _context.Groups
                .OrderBy(g => g.CreationDate)
                .Select(g => new GroupDto(g.Id, g.Name, g.Description))
                .ToListAsync();
            return new GetGroupsResponse(groups);
        }
    }

    public class UserGroupDto
    {
        public UserGroupDto(string name, string description, List<MemberDto> members)
        {
            Name = name;
            Description = description;
            Members = members;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<MemberDto> Members { get; set; }
    }

    public class MemberDto
    {
        public MemberDto(int id, string name, string email, string rule)
        {
            Id = id;
            Name = name;
            Email = email;
            Rule = rule;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Rule { get; set; }
    }

    public class GetGroupsResponse
    {
        public GetGroupsResponse(List<GroupDto> groups)
        {
            Groups = groups;
        }
        public List<GroupDto> Groups { get; set; }
    }

    public class GroupDto
    {
        public GroupDto(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
