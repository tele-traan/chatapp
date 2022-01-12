using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
        public ManageController(IUsersRepository repo, IRoomsRepository roomsRepo)
        {
            _usersRepo = repo;
            _roomsRepo = roomsRepo;
        }

        public IActionResult Index(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            var managedRooms = user.ManagedRooms.AsReadOnly();
            return View(new ManageViewModel { Message=msg, ManagedRooms = managedRooms});
        }

        public IActionResult ManageRoom(int roomId)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            Room room = _roomsRepo.GetRoom(roomId);
            if (room.ContainsAdmin(User.Identity.Name))
            {
                HttpContext.Session.SetString("currentlymanagedroom", room.Name);
                return View(model: room);
            }
            else return RedirectToAction(actionName:"Index", new {msg="Ошибка. У вас нет доступа к управлению этой комнатой"});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsername(ManageViewModel model)
        {
            string msg;
            if (ModelState.IsValid)
            {
                string prevUserName = User.Identity.Name;
                var user = _usersRepo.GetUser(prevUserName, model.Password);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    _usersRepo.UpdateUser(user);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await this.Authenticate(model.UserName);
                    msg = "Имя успешно изменено";
                }
                else msg = "Ошибка. Проверьте правильность введённых данных";
            }
            else msg = "Ошибка. Проверьте, все ли поля формы вы заполнили";
            return RedirectToAction(actionName: "Index", controllerName: "Manage", new { msg });
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
                bool isDataCorrect = this.CompareHashes(model.OldPassword, user.Salt, user.PasswordHash);
                if (isDataCorrect)
                {

                    user.PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: model.NewPassword,
                        salt: user.Salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                        ));
                    _usersRepo.UpdateUser(user);
                    msg = "Пароль успешно изменён";
                }
                else msg = "Ошибка. Проверьте правильность введённых данных";
            }
            else msg = "Ошибка. Проверьте, все ли поля формы вы заполнили";
            return RedirectToAction(actionName: "Index", controllerName: "Manage", new { msg });
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(actionName: "Login", controllerName: "Auth");
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
                return RedirectToAction(actionName: "Register", controllerName: "Auth", new { msg = "Аккаунт успешно удалён." });
            }
            else return RedirectToAction(actionName: "Login", controllerName: "Auth", new { msg = "Ошибка. Войдите снова" });
        }
    }
}