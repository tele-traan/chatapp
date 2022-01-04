using ChatApp.DB;
using ChatApp.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Linq;


namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly ChatDbContext _dbContext;
        public HomeController(ChatDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index(string msg)
        {
            ViewData["Username"] = User.Identity.Name;
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
            int users = _dbContext.GlobalChatUsers.Count();
            int rusers = _dbContext.RoomUsers.Count();
            var model = new BaseViewModel { 
                Message = msg ?? $"На данный момент в основном чате {users} человек, в комнатах {rusers} человек", 
                UserName = User.Identity.Name};
            return View(model);
        }
        public IActionResult Chat()
        {
            ViewData["Username"] = User.Identity.Name;
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if(user.GlobalChatUser!=null)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(actionName: "Login", controllerName: "Auth", new { msg = "Пользователь уже в сети" });
            }
            var users = _dbContext.GlobalChatUsers.ToList();
            var obj = new ChatViewModel { Users = users };
           return View(obj);
        }
    }
}