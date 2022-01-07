using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using ChatApp.DB;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly IUsersRepository _usersRepo;
        private readonly IRoomsRepository _roomsRepo;
        public RoomController(IUsersRepository repo, IRoomsRepository roomsRepo)
        {
            _usersRepo = repo;
            _roomsRepo = roomsRepo;
        }
        public IActionResult RoomIndex(string type, string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName); 
            if (user.RoomUser!=null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home", new { msg = "Пользователь уже в сети" });
            }
            var roomList = _roomsRepo.GetAllRooms().ToList();
            return View(new RoomViewModel { Type=type, Message = msg, Rooms = roomList });
        }
        public IActionResult Create(RoomViewModel model)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var room = _roomsRepo.GetRoom(model.RoomName);
            if (room == null)
            {
                room = new() { Name = model.RoomName };
                _roomsRepo.AddRoom(room);

                var user = _usersRepo.GetUser(userName);
                user.RoomUser = new() { Room = room, UserName = userName, IsAdmin = true };
                _usersRepo.UpdateUser(user);
                room = _roomsRepo.GetRoom(model.RoomName);
                room.Users.Add(user.RoomUser);
                room.Admins.Add(user);
                _roomsRepo.UpdateRoom(room);

                var obj = new RoomViewModel
                {
                    UserName = userName,
                    RoomName = model.RoomName,
                    Message = $"Комната {model.RoomName}",
                    UsersInRoom = room.Users,
                    RoomAdmins = room.Admins
                };
                return View(viewName: "Index", obj);
            }
            else
            {
                return RedirectToAction
                    (actionName: "RoomIndex", controllerName: "Room", new { msg = "Комната с таким названием уже существует", type="create" });
            }
        }
        public IActionResult Connect(RoomViewModel model)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var room = _roomsRepo.GetRoom(model.RoomName);
            var user = _usersRepo.GetUser(userName);
            if (room != null)
            {
                user.RoomUser = new() { Room = room, UserName = userName };
                _usersRepo.UpdateUser(user);
                var list = room.Users;
                list.Remove(user.RoomUser);
                var obj = new RoomViewModel { UserName = model.UserName, RoomName = model.RoomName, UsersInRoom = list };
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