using DatingApplication.Data;
using DatingApplication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApplication.Hubs
{

    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendNotification(string userId, string message)
        {
            // Get sender's details
            var senderId = Context.UserIdentifier; // or fetch the sender's ID in another way
            var sender = await _context.Users.FindAsync(senderId);
            var senderName = sender?.UserName; // or sender.Email if preferred
            var createdAt = DateTime.UtcNow;

            // Save notification to the database
            var notification = new Notification
            {
                UserId = userId,
                Message = $"{senderName}: {message}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send notification to the user via SignalR
            await Clients.User(userId).SendAsync("ReceiveNotification", senderName, message);
        }

    }
}