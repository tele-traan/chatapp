using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBContent _dbContext;
        public AuthController(DBContent context)
        {
            _dbContext = context;
        }
        public IActionResult Register(string msg)
        {
            string userName = User.Identity.Name;
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            if (userName != null && user != null && !user.OnlineNow) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new RegisterViewModel { Message = msg });
        }
        public IActionResult Login(string msg)
        {
            string userName = User.Identity.Name;
            var user = _dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            if (userName!= null && user!=null && !user.OnlineNow) return RedirectToAction(actionName: "Index", controllerName: "Home");
            return View(new LoginViewModel { Message = msg });
        }
        public IActionResult Manage(string msg) => View(new LoginViewModel { Message=msg});

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbContext.RegularUsers.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                if (user == null)
                {
                    _dbContext.RegularUsers.Add(new RegularUser 
                    { UserName = model.UserName, 
                        Password = model.Password, 
                        GlobalChatUser=null,
                        RoomUser=null});
                    await _dbContext.SaveChangesAsync();
                    await Authenticate(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else ModelState.AddModelError("", "Этот ник уже занят");
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            else
            {
                string msg = "";
                foreach(var v in ModelState)
                {
                    msg += v.Value;
                }
                return RedirectToAction("Register");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbContext.RegularUsers.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == model.Password);
                if (user != null&& !user.OnlineNow)
                {
                    await Authenticate(model.UserName);
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else ModelState.AddModelError("", "Неверный логин или пароль");
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbContext
                    .RegularUsers
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == model.Password);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    await _dbContext.SaveChangesAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await Authenticate(model.UserName);
                }
                else return RedirectToAction(actionName: "Index", controllerName: "Home", new {});
            }
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(actionName: "Login");
        }
    }
}