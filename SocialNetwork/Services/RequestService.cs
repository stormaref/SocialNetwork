using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.Entities;
using SocialNetwork.Infrastructure.Persistence;

namespace SocialNetwork.Services
{
    public interface IRequestService
    {
        Task<JoinRequestVM> GetUserRequests(int userId);
        Task<bool> Request(int userId, int groupId);
        Task<JoinRequestVM?> GetGroupRequests(int userId);
        Task<bool> Accept(int userId, int joinRequestId);
    }

    public class RequestService : IRequestService
    {
        private readonly DatabaseContext _context;
        private readonly IGroupService _groupService;

        public RequestService(DatabaseContext context, IGroupService groupService)
        {
            _context = context;
            _groupService = groupService;
        }

        public async Task<bool> Accept(int myUserId, int joinRequestId)
        {
            var request = await _context.JoinRequests
                .Include(r => r.Group)
                .SingleOrDefaultAsync(r => r.Id == joinRequestId);

            if (request == null)
                return false;

            var userId = request.UserId;

            var group = request.Group;
            if (request.Group.AdminId != myUserId)
                return false;

            await _context.Entry(group)
                .Collection(g => g.Members).LoadAsync();

            if (group.Members.Any(m => m.Id == userId))
                return false;

            var user = await _context.Users.FindAsync(userId);
            user.JoinDate = DateTime.Now;

            group.Members.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JoinRequestVM?> GetGroupRequests(int userId)
        {
            var group = await _context.Groups
                .SingleOrDefaultAsync(g => g.AdminId == userId);

            if (group == null)
            {
                return null;
            }

            var dtos = await _context.JoinRequests
                .Where(j => j.GroupId == group.Id)
                .OrderBy(j => j.CreationDate)
                .Select(j => new JoinRequestDto(j.Id, j.UserId, j.GroupId, j.Date))
                .ToListAsync();

            return new JoinRequestVM(dtos);
        }

        public async Task<JoinRequestVM> GetUserRequests(int userId)
        {
            var dtos = await _context.JoinRequests
                .Where(j => j.UserId == userId)
                .OrderBy(j => j.CreationDate)
                .Select(j => new JoinRequestDto(j.Id, j.UserId, j.GroupId, j.Date))
                .ToListAsync();

            return new JoinRequestVM(dtos);
        }

        public async Task<bool> Request(int userId, int groupId)
        {
            if (await _groupService.CheckUserGroup(userId))
            {
                return false;
            }
            _context.JoinRequests.Add(new JoinRequest(groupId, userId));
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class JoinRequestVM
    {
        public JoinRequestVM(List<JoinRequestDto> joinRequests)
        {
            JoinRequests = joinRequests;
        }

        public List<JoinRequestDto> JoinRequests { get; set; }
    }

    public class JoinRequestDto
    {
        public JoinRequestDto(int id, int userId, int groupId, long date)
        {
            Id = id;
            UserId = userId;
            GroupId = groupId;
            Date = date;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public long Date { get; set; }
    }
}
