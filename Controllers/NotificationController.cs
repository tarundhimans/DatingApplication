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

            var notificationList = notifications.Select(n => new
            {
                id = n.Id,
                message = n.Message
            }).ToList();

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
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var notification = await _context.Notifications
                                             .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(); // Return 404 if the notification is not found
            }

            // Optionally, mark the notification as read
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }

            // Retrieve sender profile details if SenderId is provided
            Profile senderProfile = null;
            if (!string.IsNullOrEmpty(notification.SenderId))
            {
                senderProfile = await _context.Profiles
                                              .Include(p => p.User) // To include user details if needed
                                              .FirstOrDefaultAsync(p => p.UserId == notification.SenderId);
            }

            // Prepare ViewModel
            var viewModel = new NotificationDetailsViewModel
            {
                Notification = notification,
                SenderProfile = senderProfile
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Like(int notificationId, string senderId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Create a new match entry if needed
                var match = new Match
                {
                    UserId = currentUserId,
                    MatchedUserId = senderId,
                    IsLiked = true
                };
                _context.Matches.Add(match);
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                // Redirect to chat room
                return RedirectToAction("Room", "Chat", new { userId1 = currentUserId, userId2 = senderId });
            }
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Dislike(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }



}


