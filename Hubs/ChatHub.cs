using DatingApplication.Data;
using DatingApplication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApplication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier;
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            // Save message to the database
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Send the message to the receiver
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content, message.Timestamp);
            // Send the message to the sender as well
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, content, message.Timestamp);
        }
        public async Task SendNotification(string userId, string message)
        {
            var senderId = Context.UserIdentifier;
            var sender = await _context.Users.FindAsync(senderId);
            var senderProfile = await _context.Profiles.Where(p => p.UserId == senderId).Include(p => p.User).FirstOrDefaultAsync();

            var createdAt = DateTime.UtcNow;

            // Save notification to the database
            var notification = new Notification
            {
                UserId = userId,
                SenderId = senderId,
                Message = $"{senderProfile?.User.Name}: {message}",
                CreatedAt = createdAt,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send notification to the user via SignalR, include profile info
            await Clients.User(userId).SendAsync("ReceiveNotification", senderProfile?.User.Name, message, createdAt, senderProfile);
        }

        public async Task LikeAndCreateChatRoom(string receiverId)
        {
            var senderId = Context.UserIdentifier;

            // Create a unique chat room name
            var chatRoomName = $"{senderId}-{receiverId}";

            // Create chat room
            await CreateChatRoom(senderId, receiverId);

            // Notify the receiver about the new chat room
            await Clients.User(receiverId).SendAsync("ChatRoomOpened", chatRoomName);
        }

        public async Task CreateChatRoom(string userId1, string userId2)
        {
            var groupName = $"{userId1}-{userId2}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ChatRoomOpened", groupName);
        }
    }
}
