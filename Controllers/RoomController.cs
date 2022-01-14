using System;
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
using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly IUsersRepository _usersRepo;
        private readonly IRoomsRepository _roomsRepo;
        private readonly IBanInfoRepository _bansRepo;
        public RoomController(IUsersRepository repo, IRoomsRepository roomsRepo, IBanInfoRepository bansRepo)
        {
            _usersRepo = repo;
            _roomsRepo = roomsRepo;
            _bansRepo = bansRepo;
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Index([FromForm][FromQuery] string type, [FromForm]string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user is null)
                return this.RedirectToPostAction(actionName: "Register",
                    controllerName: "Auth",
                    new() { {"msg", "Ошибка. Войдите снова" } });
            //return RedirectToAction(actionName:"Register", controllerName: "Auth", new { msg = "Ошибка. Войдите снова"});
            if (user.RoomUser is not null)
                return this.RedirectToPostAction(actionName: "Index",
                    controllerName: "Home",
                    new() { { "msg", "С одного аккаунта можно находиться только в одной комнате" } });
                //return RedirectToAction(actionName: "Index", controllerName: "Home", new { msg = "Этот аккаунт уже находится в комнате. Кикнуть?" });
            
            var roomList = _roomsRepo.GetAllRooms().ToList();
            return View(new RoomViewModel { Type=type, Message = msg, Rooms = roomList });
        }
        public IActionResult Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name;
                ViewData["Username"] = userName;
                var room = _roomsRepo.GetRoom(model.RoomName);
                if (room is null)
                {
                    if (model.IsPrivate && model.RoomPassword is null)
                        return this.RedirectToPostAction(actionName: "Index",
                            controllerName: "Room",
                            new() { { "type", "create" }, { "msg", "Вы не ввели пароль от комнаты" } });
                    /* return RedirectToAction(actionName: "Index",
                         controllerName: "Room",
                         new { type = "create", msg = "Вы не ввели пароль для комнаты" });*/

                    var user = _usersRepo.GetUser(userName);
                    room = new() { Name = model.RoomName, Creator = user };
                    if (model.IsPrivate)
                    {
                        room.IsPrivate = true;
                        room.PasswordHash = model.RoomPassword;
                    }
                    _roomsRepo.AddRoom(room);

                    user.RoomUser = new() { Room = room, UserName = userName, IsAdmin = true };
                    _usersRepo.UpdateUser(user);
                    room = _roomsRepo.GetRoom(model.RoomName);
                    room.Admins.Add(user);
                    _roomsRepo.UpdateRoom(room);

                    var list = room.RoomUsers;
                    list.Remove(list.FirstOrDefault(u => u.Equals(user.RoomUser)));
                    var obj = new RoomViewModel
                    {
                        UserName = userName,
                        RoomName = model.RoomName,
                        Message = $"Комната {model.RoomName}",
                        UsersInRoom = list,
                        RoomAdmins = room.Admins
                    };
                    return View(viewName: "Room", obj);
                }
                else return this.RedirectToPostAction(actionName: "Index",
                    controllerName: "Room",
                    new() { { "type", "create" }, { "msg", "Комнаты с таким названием не существует" } });
                //return RedirectToAction(actionName: "Index", controllerName: "Room", new { msg = "Комната с таким названием уже существует", type = "create" });

            }
            else return this.RedirectToPostAction(actionName: "Index",
                controllerName: "Room",
                new() { { "msg", "Ошибка. Проверьте, все ли поля формы вы заполнили" } });
                //return RedirectToAction (actionName: "Index", controllerName: "Room", new { msg = "Ошибка. Проверьте, все ли поля формы вы заполнили" });
        }
        public IActionResult Connect(RoomViewModel model)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var room = _roomsRepo.GetRoom(model.RoomName);
            var user = _usersRepo.GetUser(userName);
            if (room is not null)
            {
                if (room.ContainsBanned(userName))
                {
                    var banInfo = _bansRepo.GetBanInfo(user, room);
                    var until = banInfo.Until;
                    int isUnbannedAlready = DateTime.Compare(DateTime.Now, until);
                    if (isUnbannedAlready < 0)
                    {
                        string msg = $"Вы забанены в комнате {room.Name} по причине \"{banInfo.Reason}\" до {until.ToShortDateString()} {until.ToShortTimeString()} администратором {banInfo.PunisherName}";
                        return this.RedirectToPostAction(actionName: "Index",
                            controllerName: "Room",
                            new() { { "msg", msg } });
                        /*return RedirectToAction(actionName: "Index",
                            controllerName: "Room",
                            new { msg = $"Вы забанены в комнате {room.Name} по причине {banInfo.Reason} до {until.ToShortDateString()} {until.ToShortTimeString()}" });*/
                    }
                    else
                    {
                        _bansRepo.RemoveBanInfo(banInfo);
                        room.BannedUsers.Remove(user);
                        _roomsRepo.UpdateRoom(room);
                    }
                }
                if (room.IsPrivate && model.RoomPassword is not null)
                {

                }
                else if(room.IsPrivate) return this.RedirectToPostAction(actionName: "Index", 
                    controllerName: "Room", 
                    new(){ { "msg", "Вы не ввели пароль" } });
                bool isAdmin = room.ContainsAdmin(userName);
                user.RoomUser = new() { Room = room, UserName = userName, IsAdmin = isAdmin };
                _usersRepo.UpdateUser(user);
                var list = room.RoomUsers;
                list.Remove(list.FirstOrDefault(u=>u.Equals(user.RoomUser)));
                var obj = new RoomViewModel { 
                    UserName = model.UserName, 
                    RoomName = model.RoomName, 
                    UsersInRoom = list,
                    RoomAdmins = room.Admins};
                return View(viewName: "Room", obj);
            }
            else
            {
                return this.RedirectToPostAction(actionName: "Index", 
                    controllerName: "Room", 
                    new(){ { "msg", "Комнаты с таким названием не существует" } });
            }
        }
    }
}