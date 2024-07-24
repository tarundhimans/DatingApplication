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
                                             .Include(n => n.User) // Ensure you include the sender's details
                                             .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(); // Returns a 404 status if the notification is not found
            }

            // Optionally, mark as read here if needed
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }

            return View(notification); // Returns the view with the notification details
        }



        [HttpPost]
        public async Task<IActionResult> ApproveLike(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            
            return RedirectToAction("Index"); // Redirect to the notification list or another relevant page
        }

        [HttpPost]
        public async Task<IActionResult> DeclineLike(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            
            return RedirectToAction("Index"); // Redirect to the notification list or another relevant page
        }



    }

}
