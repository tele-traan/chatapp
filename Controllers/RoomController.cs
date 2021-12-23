using Microsoft.AspNetCore.Mvc;
using ChatApp.DB;
using ChatApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class RoomController : Controller
    {
        private readonly DBContent _dbContent;

        public RoomController(DBContent dbContent)
        {
            _dbContent = dbContent;
        }
#nullable enable
        public IActionResult RoomIndex(string type, string? msg)
        {
            var roomList = _dbContent.Rooms.Include(r => r.Users).ToList();
            return View(new RoomViewModel { Type=type, Message = msg, Rooms = roomList });
        }
#nullable disable
        public IActionResult Create(RoomViewModel model)
        {
            ISession session = HttpContext.Session;
            string userName = session.GetString("UserName");
            var room = _dbContent.Rooms.FirstOrDefault(m => m.Name == model.RoomName);
            if (room == null)
            {
                var user = _dbContent.RegularUsers.FirstOrDefault(u => u.UserName == session.GetString("UserName"));
                room = new() { Name = model.RoomName, Users=new() };
                if(user.RoomUser==null) user.RoomUser = new() { Room = room, UserName = session.GetString("UserName") };
                _dbContent.SaveChanges();

                var obj = new RoomViewModel 
                { UserName = model.UserName, 
                    RoomName = model.RoomName, 
                    Message=$"Комната {model.RoomName}", 
                    UsersInRoom=new() { user.RoomUser } };
                return View(viewName: "Index", obj);
            }
            else
            {
                return RedirectToAction
                    (actionName: "RoomIndex", controllerName: "Room", new { Message = "Комната с таким названием уже существует" });
            }
        }
        public IActionResult Connect(RoomViewModel model)
        {
            ISession session = HttpContext.Session;
            string userName = session.GetString("UserName");
            var room = _dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == model.RoomName);
            var user = _dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            if (room != null)
            {
                user.RoomUser = new() { Room = room, UserName = userName };
                _dbContent.SaveChanges();
                var obj = new RoomViewModel { UserName = model.UserName, RoomName = model.RoomName, UsersInRoom = room.Users.ToList() };
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