using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.Entities;
using SocialNetwork.Infrastructure.Persistence;

namespace SocialNetwork.Services
{
    public interface IConnectionService
    {
        Task<ConnectionRequestVM?> GetMyGroupConnectionRequests(int userId);
        Task<bool> Send(int userId, int groupId);
        Task<bool> Accept(int userId, int groupId);
    }
    public class ConnectionService : IConnectionService
    {
        private readonly DatabaseContext _context;

        public ConnectionService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Send(int userId, int groupId)
        {
            var group = await _context.Groups
                .SingleOrDefaultAsync(g => g.AdminId == userId);

            if (group == null || group.Id == groupId)
                return false;

            var targetGroup = await _context.Groups
                .FindAsync(groupId);

            if (targetGroup == null)
            {
                return false;
            }

            _context.ConnectionRequests.Add(new ConnectionRequest(group.Id, groupId));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ConnectionRequestVM?> GetMyGroupConnectionRequests(int userId)
        {
            var group = await _context.Groups
                .SingleOrDefaultAsync(g => g.AdminId == userId);

            if (group == null)
                return null;

            var dtos = await _context.ConnectionRequests
                .Where(c => c.ReceiverGroupId == group.Id)
                .OrderByDescending(c => c.CreationDate)
                .Select(c => new ConnectionRequestDto(c.Id, c.SenderGroupId, c.Sent))
                .ToListAsync();

            return new ConnectionRequestVM(dtos);
        }

        public async Task<bool> Accept(int userId, int groupId)
        {
            var myGroup = await _context.Groups
                .Include(g => g.ConnectedGroups)
                .SingleOrDefaultAsync(g => g.AdminId == userId);

            if (myGroup == null)
                return false;

            var senderGroup = await _context.Groups
                .Include(g => g.ConnectedGroups)
                .SingleOrDefaultAsync(g => g.Id == groupId);

            if (senderGroup == null)
                return false;

            var cr = await _context.ConnectionRequests
                .FirstOrDefaultAsync(c => c.SenderGroupId == groupId && c.ReceiverGroupId == myGroup.Id);

            if (cr == null)
                return false;


            myGroup.ConnectedGroups.Add(senderGroup);
            senderGroup.ConnectedGroups.Add(myGroup);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ConnectionRequestVM
    {
        public ConnectionRequestVM(List<ConnectionRequestDto> requests)
        {
            Requests = requests;
        }

        public List<ConnectionRequestDto> Requests { get; set; }
    }

    public class ConnectionRequestDto
    {
        public ConnectionRequestDto(int connectionRequestId, int groupId, long sent)
        {
            ConnectionRequestId = connectionRequestId;
            GroupId = groupId;
            Sent = sent;
        }

        public int ConnectionRequestId { get; set; }
        public int GroupId { get; set; }
        public long Sent { get; set; }
    }
}
