using ChatApp.DB;
using ChatApp.Models;
using ChatApp.Repositories;

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

        private readonly IUsersRepository _usersRepo;
        private readonly IGCUsersRepository _gcUsersRepo;
        private readonly IRoomUsersRepository _roomUsersRepo;
        public HomeController(IUsersRepository repo, IGCUsersRepository gcRepo, IRoomUsersRepository roomRepo)
        {
            _usersRepo = repo;
            _gcUsersRepo = gcRepo;
            _roomUsersRepo = roomRepo;
        }
        public IActionResult Index(string msg)
        {
            ViewData["Username"] = User.Identity.Name;
            int gcUsersCount = _gcUsersRepo.GetAllUsers().Count();
            int roomUsersCount = _roomUsersRepo.GetAllUsers().Count();
            var model = new BaseViewModel { 
                Message = msg ?? $"На данный момент в основном чате {gcUsersCount} человек, в комнатах {roomUsersCount} человек"};
            return View(model);
        }
        public IActionResult Chat()
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if(user.GlobalChatUser!=null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home", new { msg = "Пользователь уже в сети" });
            }
            var users = _gcUsersRepo.GetAllUsers().ToList();
            var obj = new ChatViewModel { Users = users };
           return View(obj);
        }
    }
}