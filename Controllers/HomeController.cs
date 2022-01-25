using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using ChatApp.Hubs;
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
        private readonly IHubContext<ChatHub> _chatHub;
        public HomeController(IUsersRepository repo,
            IGCUsersRepository gcUsersRepo,
            IRoomUsersRepository roomRepo,
            IGCRepository gcRepo,
            IHubContext<ChatHub> chatHub)
        {
            _usersRepo = repo;
            _gcUsersRepo = gcUsersRepo;
            _roomUsersRepo = roomRepo;
            _gcRepo = gcRepo;
            _chatHub = chatHub;
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Index([FromForm]string msg)
        {
            var model = new BaseViewModel { 
                Message = msg ?? $"Привет, {User.Identity.Name}"};
            return View(model);
        }
        public async Task<IActionResult> Chat()
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            var users = _gcUsersRepo.GetAllUsers().ToList();
            if (user.GlobalChatUser is not null)
            {
                Task t = _chatHub.Clients.
                    Client(user.GlobalChatUser.ConnectionId).SendAsync("ThisAccOnNewab");
                t.Wait();
                await _chatHub.Clients.All.SendAsync("MemberLeft", user.UserName);

                var gcUser = users.FirstOrDefault(u => u.Equals(user.GlobalChatUser));
                users.Remove(gcUser);
                user.GlobalChatUser = null;
            }
            var msgs = _gcRepo.GetLastMessages().ToList();
            var obj = new ChatViewModel { Users = users, LastMessages = msgs };
           return View(obj);
        }
    }
}