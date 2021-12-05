using Microsoft.AspNetCore.Mvc;
using ChatApp.DB;
using ChatApp.Models;
using ChatApp.Hubs;
using ChatApp.Util;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace ChatApp.Controllers
{
    public class RoomController : Controller
    {
        private readonly IHubContext<RoomHub> _roomHub;
        private readonly DBContent _dbContent;

        public RoomController(IHubContext<RoomHub> roomHub, DBContent dbContent)
        {
            _roomHub = roomHub;
            _dbContent = dbContent;
        }
        public IActionResult RoomIndex(string type) => View(new RoomViewModel { Message=type});
        public IActionResult Create(RoomViewModel model)
        {
            var room = _dbContent.Rooms.FirstOrDefault(m => m.Name == model.RoomName);
            if (room == null)
            {
                room = new() { Name = model.RoomName };
                _dbContent.Rooms.Add(room);
                _dbContent.SaveChanges();
                var obj = new RoomViewModel { UserName = model.UserName, RoomName = model.RoomName, IsAdmin = true };
                return View(viewName: "Index", obj);
            }
            else
            {
                return RedirectToAction
                    (actionName: "Index", controllerName: "Home", new { Message = "Комната с таким названием уже существует" });
            }
        }
        public IActionResult Connect(RoomViewModel model)
        {
            var room = _dbContent.Rooms.FirstOrDefault(r => r.Name == model.RoomName);
            if (room != null)
            {
                _roomHub.Clients.All//Except(this.GetExcluded(model.RoomName))
                    .SendAsync("MemberJoined", model.UserName);
                room.Users.Add(new User { Username = model.UserName});
                _dbContent.SaveChanges();
                var obj = new RoomViewModel { UserName = model.UserName, RoomName = model.RoomName, IsAdmin = false };
                return View(viewName: "Index", obj);
            }
            else 
            {
                return RedirectToAction
                    (actionName: "Index", controllerName: "Home", new { Message = "Комнаты с таким названием не существует" });
            }
        }
    }
}