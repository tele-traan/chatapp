using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsersRepository _usersRepo;
        public AuthController(IUsersRepository repo)
        {
            _usersRepo = repo;
        }
        [HttpGet]
        [HttpPost]
        public IActionResult RegisterIndex([FromForm]string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user != null) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new RegisterViewModel { Message = msg });
        }
        [HttpGet]
        [HttpPost]
        public IActionResult LoginIndex([FromForm]string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user!=null) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new LoginViewModel { Message = msg });
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Manage([FromForm]string msg)
        {
            ViewData["Username"] = User.Identity.Name;
            return View(new LoginViewModel { Message = msg });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _usersRepo.GetUser(model.UserName);
                if (user == null)
                {
                    _usersRepo.AddUser(new User
                    {
                        PasswordHash = model.Password,
                        UserName = model.UserName,
                        GlobalChatUser = null,
                        RoomUser = null
                    });
                    await this.Authenticate(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else return this.RedirectToPostAction(actionName: "RegisterIndex",
                    controllerName: "Auth",
                    new() { { "msg", "Этот ник уже занят" } });
            }
            else return this.RedirectToPostAction(actionName: "Register",
                controllerName: "Auth",
                new() { { "msg", "Ошибка. Проверьте, все ли поля формы вы заполнили" } }); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _usersRepo.GetUser(model.UserName);
                if (user != null)
                {
                    bool authenticated = this.GetHash(model.Password, user.Salt) == user.PasswordHash;
                    if (authenticated)
                    {
                        await this.Authenticate(model.UserName);
                        return RedirectToAction(actionName: "Index", controllerName: "Home");
                    }
                }
                return this.RedirectToPostAction(actionName: "LoginIndex",
                    controllerName: "Auth",
                    new() { { "msg", "Неверный логин или пароль" } });
            }
            else return this.RedirectToPostAction(actionName: "LoginIndex",
                controllerName: "Auth",
                new() { { "msg", "Ошибка. Проверьте, все ли поля формы вы заполнили" } });
        }
    }
}