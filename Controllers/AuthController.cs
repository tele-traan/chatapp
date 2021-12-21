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
        public IActionResult AfterChange()
        {
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
        public IActionResult Register() => View();
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Auth(AuthViewModel model)
        {
            ISession session = HttpContext.Session;
            _dbContent.RegularUsers.Add(new RegularUser { UserName = model.UserName, Password = model.Password });
            int a = _dbContent.SaveChanges();
            session.SetString("UserName", model.UserName);
            string password = model.Password;
            return RedirectToAction(actionName:"Index", controllerName:"Home", new {message=$"{a}"});
        }
    }
}
