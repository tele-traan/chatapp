using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
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
            int users = _dbContent.GlobalChatUsers.Count();
            int rusers = _dbContent.RoomUsers.Count();
            var model = new BaseViewModel { Message = message ?? 
                $"На данный момент в основном чате {users} человек, в комнатах {rusers} человек" };
            return View(model);
        }
        public async Task<IActionResult> Chat(BaseViewModel model)
        {
            string userName = model.UserName;
            ISession session = HttpContext.Session;
            session.SetString("UserName", userName);
            if (userName != null && userName != "" && userName != " ")
            {
                _dbContent.GlobalChatUsers.Add(new GlobalChatUser { UserName = userName });
                _dbContent.SaveChanges();
                await _hub.Clients.All.SendAsync("MemberJoined", userName);
            }
            return View(model);
        }
    }
}