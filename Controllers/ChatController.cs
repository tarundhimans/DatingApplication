using DatingApplication.Data;
using DatingApplication.Hubs;
using DatingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApplication.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(ApplicationDbContext context, IHubContext<ChatHub> chatHubContext)
        {
            _context = context;
            _chatHubContext = chatHubContext;
        }

        public async Task<IActionResult> Index()
        {
             if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var matches = await _context.Matches
                .Include(m => m.User)
                .Include(m => m.MatchedUser)
                .Where(m => m.UserId == userId || m.MatchedUserId == userId)
                .ToListAsync();

            return View(matches);
        }

        public async Task<IActionResult> Room(string userId1, string userId2)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId2);
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId1);

            ViewBag.UserId1 = userId1;
            ViewBag.UserId2 = userId2;
            ViewBag.Messages = messages;
            ViewBag.ReceiverName = receiver?.UserName; 
            ViewBag.SenderName = sender?.UserName; 

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Like(string receiverId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Invalid sender or receiver ID.");
            }

            var senderExists = await _context.Users.AnyAsync(u => u.Id == senderId);
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);

            if (!senderExists || !receiverExists)
            {
                return BadRequest("One or both users do not exist.");
            }

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => (m.UserId == senderId && m.MatchedUserId == receiverId) ||
                                          (m.UserId == receiverId && m.MatchedUserId == senderId));

            if (match == null)
            {
                match = new Match
                {
                    UserId = senderId,
                    MatchedUserId = receiverId,
                    IsLiked = true
                };

                _context.Matches.Add(match);
                await _context.SaveChangesAsync();
            }
            else
            {
                match.IsLiked = true;
                _context.Matches.Update(match);
                await _context.SaveChangesAsync();
            }

            // Notify both users to update their chat room lists
            await _chatHubContext.Clients.User(receiverId).SendAsync("UpdateChatRooms", senderId);
            await _chatHubContext.Clients.User(senderId).SendAsync("UpdateChatRooms", receiverId);

            return RedirectToAction("Index"); // Redirect to the chat index page
        }
    }
}
