using DatingApplication.Data;
using DatingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DatingApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profiles = await _context.Profiles.Include(p => p.User).Where(p => p.UserId != userId).ToListAsync();

            return View(profiles);
        }



        [HttpPost]
      
        public async Task<IActionResult> LikeProfile(string matchedUserId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var match = new Match
            {
                UserId = userId,
                MatchedUserId = matchedUserId,
                IsLiked = true,
                IsRejected = false
            };

            _context.Matches.Add(match);

            var notification = new Notification
            {
                UserId = matchedUserId,
                Message = "You have a new like!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
