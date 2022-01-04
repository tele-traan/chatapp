using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;


using ChatApp.DB;
using ChatApp.Util;
using ChatApp.Models;

namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ChatDbContext _dbContext;
        public AuthController(ChatDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Register(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _dbContext.GetUser(userName);
            if (user != null && !user.OnlineNow) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new RegisterViewModel { Message = msg });
        }
        public IActionResult Login(string msg)
        {
            string userName = User.Identity.Name;
            ViewData["Username"] = userName;
            var user = _dbContext.GetUser(userName);
            if (userName!= null && user!=null && !user.OnlineNow) return RedirectToAction(actionName: "Index", controllerName: "Home");
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
                var user = _dbContext.GetUser(model.UserName);
                if (user == null)
                {
                    _dbContext.RegularUsers.Add(new RegularUser 
                    { UserName = model.UserName, 
                        Password = model.Password, 
                        GlobalChatUser=null,
                        RoomUser=null});
                    await _dbContext.SaveChangesAsync();
                    await AuthenticateAsync(model.UserName);
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
                var user = _dbContext.GetUser(model.UserName, model.Password);
                if (user != null&& !user.OnlineNow)
                {
                    await AuthenticateAsync(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else return RedirectToAction(actionName:"Login", new { msg = "Неверный логин или пароль" });
            }
            return RedirectToAction("Login", new { msg = "Ошибка. Проверьте, заполнили ли вы все поля формы" });
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsername(LoginViewModel model)
        {
            string msg = "";
            if (ModelState.IsValid)
            {
                string prevUserName = User.Identity.Name;
                var user = await _dbContext
                    .RegularUsers
                    .FirstOrDefaultAsync(u => u.UserName == prevUserName);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    _dbContext.SaveChanges();
                    Task signOutTask = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    Task authTask = signOutTask.ContinueWith(t => AuthenticateAsync(model.UserName));
                    authTask.Wait();
                    msg="Имя успешно изменено";
                }
                else msg="Ошибка";
            }
            return RedirectToAction(actionName: "Index", controllerName: "Home", new { msg });
        }*/
        private async Task AuthenticateAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}