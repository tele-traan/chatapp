using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly DBContent _dbContent;
        public HomeController(IHubContext<ChatHub> hub, DBContent content)
        {
            _hub = hub; _dbContent = content;
        }
        public IActionResult Index(string message)
        {
            int users = _dbContent.Users.Count();
            var model = new BaseViewModel { Message = message ?? $"Привет! На данный момент в чате {users} человек" };
            return View(model);
        }
        public async Task<IActionResult> Chat(BaseViewModel model)
        {
            string userName = model.UserName;
            if (userName != null && userName != "" && userName != " ")
            {
                _dbContent.Users.Add(new User { Username = userName });
                _dbContent.SaveChanges();
                await _hub.Clients.All.SendAsync("MemberJoined", userName);
            }
            return View(model);
        }
    }
}