using Microsoft.AspNetCore.Mvc;

namespace DatingApplication.Controllers
{
    public class UserController : Controller
    {
        public IActionResult SelectChatUser(string receiverId)
        {
            HttpContext.Session.SetString("ReceiverId", receiverId);
            return RedirectToAction("Index", "Chat");
        }
    }

}
