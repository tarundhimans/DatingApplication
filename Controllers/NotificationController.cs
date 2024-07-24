using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DatingApplication.Models;
using System.Linq;
using DatingApplication.Data;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userId && !n.IsRead)
                                              .OrderByDescending(n => n.CreatedAt)
                                              .ToListAsync();

            var notificationList = notifications.Select(n => new { message = n.Message }).ToList();

            return Json(new { notifications = notificationList });
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userId)
                                              .OrderByDescending(n => n.CreatedAt)
                                              .ToListAsync();

            ViewBag.NotificationCount = notifications.Count;
            ViewBag.Notifications = notifications;

            return View(notifications);
        }
    }

}
