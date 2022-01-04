using ChatApp.DB;
using ChatApp.Models;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ChatApp.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly DBContent _dbContext;

        public RoomController(DBContent dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult RoomIndex(string type, string msg)
        {
            ViewData["Username"] = User.Identity.Name;
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user.RoomUser!=null)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(actionName: "Login", controllerName: "Auth", new { msg = "Пользователь уже в сети" });
            }
            var roomList = _dbContext.Rooms.Include(r => r.Users).AsNoTracking().ToList();
            return View(new RoomViewModel { Type=type, Message = msg, Rooms = roomList });
        }
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var room = await _dbContext.Rooms.FirstOrDefaultAsync(m => m.Name == model.RoomName);
            if (room == null)
            {
                room = new() { Name = model.RoomName, Users = new List<RoomUser>() };
                _dbContext.Rooms.Add(room);
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.RegularUsers
                    .Include(u => u.RoomUser)
                    .ThenInclude(ru => ru.Room)
                    .FirstOrDefaultAsync(u => u.UserName == userName);
                user.RoomUser = new() { Room = room, UserName = userName, IsAdmin = true };

                room = await _dbContext.Rooms.Include(r=>r.Users).FirstOrDefaultAsync(r => r.Name == model.RoomName);
                room.Users.Add(user.RoomUser);

                await _dbContext.SaveChangesAsync();

                var obj = new RoomViewModel
                {
                    UserName = userName,
                    RoomName = model.RoomName,
                    Message = $"Комната {model.RoomName}",
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
            var room = _dbContext.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == model.RoomName);
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            if (room != null)
            {
                user.RoomUser = new() { Room = room, UserName = userName };
                _dbContext.SaveChanges();
                var list = room.Users.ToList();
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