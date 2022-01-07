using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

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
        public IActionResult Register(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user != null) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new RegisterViewModel { Message = msg });
        }
        public IActionResult Login(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _usersRepo.GetUser(userName);
            if (user!=null) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new LoginViewModel { Message = msg });
        }
        public IActionResult Manage(string msg)
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
                    { UserName = model.UserName, 
                        Password = model.Password,
                        GlobalChatUser=null,
                        RoomUser=null});
                    await this.Authenticate(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else return RedirectToAction(actionName: "Register", new {msg="Этот ник уже занят"});
            }
            else return RedirectToAction(actionName:"Register", new {msg="Ошибка. Проверьте, заполнили ли вы все поля формы"});
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _usersRepo.GetUser(model.UserName, model.Password);
                if (user != null)
                {
                    await this.Authenticate(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else return RedirectToAction(actionName: "Login", new { msg = "Неверный логин или пароль" });
            }
            else return RedirectToAction("Login", new { msg = "Ошибка. Проверьте, заполнили ли вы все поля формы" });
        }
    }
}