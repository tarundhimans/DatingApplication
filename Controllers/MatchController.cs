//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using DatingApplication.Models;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using DatingApplication.Data;
//using DatingApplication.Hubs;

//public class MatchController : Controller
//{
//    private readonly IHubContext<ChatHub> _hubContext;
//    private readonly ApplicationDbContext _context;

//    public MatchController(IHubContext<ChatHub> hubContext, ApplicationDbContext context)
//    {
//        _hubContext = hubContext;
//        _context = context;
//    }

//    [HttpPost]
//    public async Task<IActionResult> LikeProfile(int profileId)
//    {
//        var profile = await _context.Profiles.FindAsync(profileId);
//        if (profile == null)
//        {
//            return NotFound();
//        }

//        var match = await _context.Matches
//            .FirstOrDefaultAsync(m => m.UserId == User.Identity.Name && m.MatchedUserId == profile.UserId);

//        if (match == null)
//        {
//            match = new Match
//            {
//                UserId = User.Identity.Name,
//                MatchedUserId = profile.UserId,
//                IsLiked = true,
//                IsRejected = false
//            };
//            _context.Matches.Add(match);
//        }
//        else
//        {
//            match.IsLiked = true;
//            match.IsRejected = false;
//            _context.Matches.Update(match);
//        }
//        await _context.SaveChangesAsync();

//        // Send notification
//        await _hubContext.Clients.User(profile.UserId).SendAsync("ReceiveNotification", "You have a new like!");

//        return Ok();
//    }

//    [HttpPost]
//    public async Task<IActionResult> DislikeProfile(int profileId)
//    {
//        var profile = await _context.Profiles.FindAsync(profileId);
//        if (profile == null)
//        {
//            return NotFound();
//        }

//        var match = await _context.Matches
//            .FirstOrDefaultAsync(m => m.UserId == User.Identity.Name && m.MatchedUserId == profile.UserId);

//        if (match == null)
//        {
//            match = new Match
//            {
//                UserId = User.Identity.Name,
//                MatchedUserId = profile.UserId,
//                IsLiked = false,
//                IsRejected = true
//            };
//            _context.Matches.Add(match);
//        }
//        else
//        {
//            match.IsLiked = false;
//            match.IsRejected = true;
//            _context.Matches.Update(match);
//        }
//        await _context.SaveChangesAsync();

//        // Send notification
//        await _hubContext.Clients.User(profile.UserId).SendAsync("ReceiveNotification", "You have a new dislike!");

//        return Ok();
//    }
//}
