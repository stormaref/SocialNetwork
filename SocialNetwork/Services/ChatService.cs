using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.Entities;
using SocialNetwork.Infrastructure.Persistence;

namespace SocialNetwork.Services
{
    public interface IChatService
    {
        Task<ChatsVm?> GetMyChats(int userId);
        Task<MessagesVm> GetMyChatWithUser(int myUserId, int targetUserId);
        Task<bool> Send(int myUserId, int targetUserId, string message);
    }
    public class ChatService : IChatService
    {
        private readonly DatabaseContext _context;
        private readonly IGroupService _groupService;

        public ChatService(DatabaseContext context, IGroupService groupService)
        {
            _context = context;
            _groupService = groupService;
        }

        public async Task<ChatsVm?> GetMyChats(int userId)
        {
            var chats = await _context.Chats
                .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
                .OrderByDescending(c => c.CreationDate)
                .ToListAsync();

            List<ChatsDto> ChatsDto = new List<ChatsDto>();
            foreach (var c in chats)
            {
                var dto = new ChatsDto(GetOtherUserId(c, userId), await GetOtherUserNameAsync(c, userId));
                ChatsDto.Add(dto);
            }

            return new ChatsVm(ChatsDto);
        }

        private async Task<string> GetOtherUserNameAsync(Chat c, int userId)
        {
            var otherUserId = GetOtherUserId(c, userId);
            var user = await _context.Users
                .FindAsync(otherUserId);
            return user.Name;
        }

        private int GetOtherUserId(Chat c, int userId)
        {
            return c.FirstUserId == userId ? c.SecondUserId : c.FirstUserId;
        }

        public async Task<MessagesVm> GetMyChatWithUser(int myUserId, int targetUserId)
        {
            var chat = await GetMyChatWith(myUserId, targetUserId);

            if (chat == null)
            {
                return null;
            }

            var dtos = chat.Messages
                .Select(m => new MessagesDto(m.Body, m.Date, m.SenderId))
                .OrderByDescending(m => m.Date)
                .ToList();

            return new MessagesVm(dtos);
        }

        private async Task<Chat?> GetMyChatWith(int myUserId, int targetUserId)
        {
            return await _context.Chats
                            .Include(c => c.Messages)
                            .SingleOrDefaultAsync(c =>
                            (c.FirstUserId == myUserId && c.SecondUserId == targetUserId) ||
                            (c.SecondUserId == myUserId && c.FirstUserId == targetUserId));
        }

        public async Task<bool> Send(int myUserId, int targetUserId, string message)
        {
            var chat = await GetMyChatWith(myUserId, targetUserId);
            if (chat == null)
                return await CreateNewChat(myUserId, targetUserId, message);

            chat.Messages.Add(new Message(myUserId, targetUserId, message));
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> CreateNewChat(int myUserId, int targetUserId, string message)
        {
            var me = await _context.Users.FindAsync(myUserId);
            if (!me.GroupId.HasValue)
                return false;
            var myGroup = await _context.Groups.FindAsync(me.GroupId);
            var him = await _context.Users.FindAsync(targetUserId);
            if (!him.GroupId.HasValue)
                return false;
            var isConnected = await _groupService.IsConnected(myGroup, him.GroupId.Value);
            if ((myGroup.Id != him.GroupId.Value) && !isConnected)
                return false;
            var chat = new Chat(myUserId, targetUserId, new Message(myUserId, targetUserId, message));
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class MessagesVm
    {
        public MessagesVm(List<MessagesDto> messages)
        {
            Messages = messages;
        }

        public List<MessagesDto> Messages { get; set; }
    }

    public class MessagesDto
    {
        public MessagesDto(string message, long date, int sentBy)
        {
            Message = message;
            Date = date;
            SentBy = sentBy;
        }

        public string Message { get; set; }
        public long Date { get; set; }
        public int SentBy { get; set; }
    }

    public class ChatsVm
    {
        public ChatsVm(List<ChatsDto> chats)
        {
            Chats = chats;
        }

        public List<ChatsDto> Chats { get; set; }
    }

    public class ChatsDto
    {
        public ChatsDto(int userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
