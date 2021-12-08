using Microsoft.AspNetCore.Mvc;
using ChatApp.DB;
using ChatApp.Models;
using ChatApp.Hubs;
using ChatApp.Util;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult RoomIndex(string type) => View(new RoomViewModel { Message = type });
        /*{

            TODO (auto generated method stub :D)
        }*/
        public IActionResult Create(RoomViewModel model)
        {
            ISession session = HttpContext.Session;
            session.SetString("UserName", model.UserName);
            session.SetString("RoomName", model.RoomName);
            var room = _dbContent.Rooms.FirstOrDefault(m => m.Name == model.RoomName);
            if (room == null)
            {
                room = new() { Name = model.RoomName, Users=new() };
                _dbContent.Rooms.Add(room);
                _dbContent.SaveChanges();

                room = _dbContent.Rooms.Include(r => r.Users).FirstOrDefault(r => r.Name == model.RoomName);
                room.Users.Add(new RoomUser { UserName = model.UserName, Room = room, RoomId = room.RoomId });
                _dbContent.SaveChanges();

                var obj = new RoomViewModel 
                { UserName = model.UserName, RoomName = model.RoomName, Message=$"Комната {model.RoomName}", IsAdmin=true };
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
            ISession session = HttpContext.Session;
            session.SetString("UserName", model.UserName);
            session.SetString("RoomName", model.RoomName);
            var room = _dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == model.RoomName);
            if (room != null)
            {
                room.Users.Add(new RoomUser { UserName = model.UserName });
                _dbContent.SaveChanges();
                _roomHub.Clients.Clients(this.GetIds(model.RoomName))
                    .SendAsync("MemberJoined", model.UserName);
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