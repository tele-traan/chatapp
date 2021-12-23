using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ChatApp.DB;
using ChatApp.Models;
using System.Linq;
namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBContent _dbContent;
        public AuthController(DBContent content)
        {
            _dbContent = content;
        }
        public IActionResult Change()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("UserName") != null)
            {

            }
            return View();
        }
        public IActionResult Register(string msg) => View(model:msg);
        public IActionResult Login(string msg) => View(model:msg);

        [HttpPost]
        public IActionResult Register(AuthViewModel model)
        {
            ISession session = HttpContext.Session;
            if (_dbContent.RegularUsers.FirstOrDefault(u => u.UserName == model.UserName)==null)
            {
                _dbContent.RegularUsers.Add(new RegularUser
                {
                    UserName = model.UserName,
                    Password = model.Password,
                });
                _dbContent.SaveChanges();
                session.SetString("UserName", model.UserName);
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            else return RedirectToAction(actionName: "Register", controllerName: "Auth", new { msg = "Неизвестная ошибка. Попробуйте снова" });
        }
        [HttpPost]
        public IActionResult Login(AuthViewModel model)
        {
            ISession session = HttpContext.Session;
            string userName = model.UserName, password = model.Password;
            var user = _dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            if (user != null && !user.OnlineNow)
            {
                session.SetString("UserName", model.UserName);
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }else if (user.OnlineNow)
            {
                return RedirectToAction(actionName: "Login", controllerName: "Auth", routeValues:new {msg="Этот пользователь уже в сети"});
            }
            else return RedirectToAction(actionName: "Login", controllerName: "Auth", routeValues:new {msg="Неверный логин или пароль. Попробуйте снова"});
        }
    }
}