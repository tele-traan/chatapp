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

        private readonly DBContent _dbContext;
        public HomeController(DBContent context)
        {
            _dbContext = context;
        }
        public IActionResult Index(string message)
        {
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user.OnlineNow)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(actionName: "Login", controllerName: "Auth", new { msg = "Пользователь уже в сети" });
            }
            int users = _dbContext.GlobalChatUsers.Count();
            int rusers = _dbContext.RoomUsers.Count();
            var model = new BaseViewModel { 
                Message = message ?? $"На данный момент в основном чате {users} человек, в комнатах {rusers} человек", 
                UserName = User.Identity.Name};
            return View(model);
        }
        public IActionResult Chat()
        {
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