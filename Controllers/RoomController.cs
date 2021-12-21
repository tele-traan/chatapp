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
        public IActionResult RoomIndex(string type)
        {
            var roomList = _dbContent.Rooms.Include(r => r.Users).ToList();
            return View(new RoomViewModel { Message = type, Rooms = roomList });
        }
        public IActionResult Create(RoomViewModel model)
        {
            ISession session = HttpContext.Session;
            session.SetString("UserName", model.UserName);
            session.SetString("IsAdmin", "true");
            session.SetString("RoomName", model.RoomName);
            var room = _dbContent.Rooms.FirstOrDefault(m => m.Name == model.RoomName);
            if (room == null)
            {
                room = new() { Name = model.RoomName, Users=new() };
                _dbContent.Rooms.Add(room);
                _dbContent.SaveChanges();
                var user = new RoomUser { UserName = model.UserName, Room = room, IsAdmin = true };
                room = _dbContent.Rooms.Include(r => r.Users).FirstOrDefault(r => r.Name == model.RoomName);
                room.Users.Add(user);
                _dbContent.SaveChanges();

                var obj = new RoomViewModel 
                { UserName = model.UserName, RoomName = model.RoomName, Message=$"Комната {model.RoomName}", UsersInRoom=new() { user } };
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
            session.SetString("IsAdmin", "false");
            session.SetString("RoomName", model.RoomName);
            var room = _dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == model.RoomName);
            if (room != null)
            {
                room.Users.Add(new RoomUser { UserName = model.UserName, IsAdmin = false });
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