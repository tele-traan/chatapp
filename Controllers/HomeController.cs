using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IGCRepository _gcRepo;
        private readonly IUsersRepository _usersRepo;
        private readonly IGCUsersRepository _gcUsersRepo;
        private readonly IRoomUsersRepository _roomUsersRepo;
        public HomeController(IUsersRepository repo, IGCUsersRepository gcUsersRepo, IRoomUsersRepository roomRepo, IGCRepository gcRepo)
        {
            _usersRepo = repo;
            _gcUsersRepo = gcUsersRepo;
            _roomUsersRepo = roomRepo;
            _gcRepo = gcRepo;
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Index([FromForm]string msg)
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
                return this.RedirectToPostAction(actionName: "Index",
                    controllerName: "Home",
                    new() { { "msg", "Пользователь уже в сети" } });
            }
            var users = _gcUsersRepo.GetAllUsers().ToList();
            var msgs = _gcRepo.GetLastMessages().ToList();
            var obj = new ChatViewModel { Users = users, LastMessages = msgs };
           return View(obj);
        }
    }
}