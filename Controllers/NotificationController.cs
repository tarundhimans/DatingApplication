using DatingApplication.Data;
using DatingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace DatingApplication.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _context.Notifications
                                             .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var unreadCount = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(unreadCount);
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userId)
                                              .OrderByDescending(n => n.CreatedAt)
                                              .ToListAsync();

            return PartialView("_NotificationsList", notifications);
        }

    }
}
