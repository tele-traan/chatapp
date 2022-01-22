using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using ChatApp.Hubs;
using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IUsersRepository _usersRepo;
        private readonly IRoomsRepository _roomsRepo;
        private readonly IHubContext<RoomHub> _roomHubContext;
        public ManageController(IUsersRepository repo, IRoomsRepository roomsRepo, IHubContext<RoomHub> _roomHub)
        {
            _usersRepo = repo;
            _roomsRepo = roomsRepo;
            _roomHubContext = _roomHub;
        }
        public IActionResult Index(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user is not null)
            {
                var managedRooms = user.ManagedRooms.AsReadOnly();
                return View(new ManageViewModel { Message = msg, ManagedRooms = managedRooms });
            }
            else return this.RedirectToPostAction(actionName: "Login",
              controllerName: "Auth",
              new() { { "msg", "Ошибка. Войдите снова" } });
        }
        public async Task<IActionResult> ManageRoom(int roomId)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            Room room = await _roomsRepo.GetRoomAsync(roomId);
            if (room is not null)
            {
                if (room.ContainsAdmin(User.Identity.Name))
                {
                    HttpContext.Session.SetString("currentlymanagedroom", room.Name);
                    return View(model: room);
                }
                else return this.RedirectToPostAction(actionName: "Index",
                    controllerName: "Manage",
                    new() { { "msg", "Ошибка. У вас нет доступа к управлению этой комнатой" } });
            }
            else return this.RedirectToPostAction(actionName: "Index",
                controllerName: "Manage",
                new() { { "msg", "Ошибка. Этой комнаты не существует. Проверьте, не меняли ли вы строку запроса" } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsername(ManageViewModel model)
        {
            string msg;
            if (ModelState.IsValid)
            {
                string prevUserName = User.Identity.Name;
                var user = _usersRepo.GetUser(prevUserName);
                if (user != null)
                {
                    if (this.GetHash(model.Password, user.Salt) == user.PasswordHash)
                    {
                        user.UserName = model.UserName;
                        _usersRepo.UpdateUser(user);
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        await this.Authenticate(model.UserName);
                        msg = "Имя успешно изменено";
                    }
                    else msg = "Ошибка. Проверьте правильность введённых данных";
                }
                else msg = "Ошибка. Проверьте правильность введённых данных";
            }
            else msg = "Ошибка. Проверьте, все ли поля формы вы заполнили";
            return this.RedirectToPostAction(actionName: "Index",
                controllerName: "Manage",
                new() { { "msg", msg } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ManageViewModel model)
        {
            string msg;
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            if (ModelState.IsValid && model.NewPassword != model.OldPassword)
            {
                var user = _usersRepo.GetUser(userName);
                bool isAuthenticated = this.GetHash(model.OldPassword, user.Salt) == user.PasswordHash;
                if (isAuthenticated)
                {
                    user.PasswordHash = model.NewPassword;
                    _usersRepo.UpdateUser(user);
                    msg = "Пароль успешно изменён";
                }
                else msg = "Ошибка. Проверьте правильность введённых данных";
            }
            else msg = "Ошибка. Проверьте, все ли поля формы вы заполнили";
            return this.RedirectToPostAction(actionName: "Index",
                controllerName: "Manage",
                new() { {"msg", msg } });
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(actionName: "LoginIndex", controllerName: "Auth");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount()
        {
            string userName = User.Identity.Name;

            var user = _usersRepo.GetUser(userName);
            if (user != null)
            {   
                _usersRepo.RemoveUser(user);
                return this.RedirectToPostAction(actionName: "RegisterIndex",
                    controllerName: "Auth",
                    new() { { "msg", "Аккаунт успешно удалён" } });
            }
            else return this.RedirectToPostAction(actionName: "LoginIndex",
                controllerName: "Auth",
                new() { {"msg", "Ошибка. Войдите снова" } });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            string userName = User.Identity.Name;
            User user = _usersRepo.GetUser(userName);
            Room room = await _roomsRepo.GetRoomAsync(roomId);
            string roomName = room.Name;
            if (room.Creator.Equals(user))
            {
                await _roomHubContext.Clients.Clients(await this.GetIds(roomName)).SendAsync("RoomDeleted");
                await _roomsRepo.RemoveRoomAsync(room);
                return this.RedirectToPostAction(actionName: "Index",
                    controllerName: "Manage",
                    new() { { "msg", $"Комната {roomName} успешно удалена" } });
            }
            else return this.RedirectToPostAction(actionName: "Index",
                controllerName: "Manage",
                new() { { "msg", "Ошибка. Удалить комнату может только её создатель" } });
        }
    }
}