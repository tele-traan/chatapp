using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IUsersRepository _usersRepo;
        public ManageController(IUsersRepository repo)
        {
            _usersRepo = repo;
        }
        public IActionResult Index(string msg)
        {
            ViewData["Username"] = User.Identity.Name;
            return View(new ManageViewModel { Message=msg});
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
            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name;
                var user = _usersRepo.GetUser(userName, model.OldPassword);
                if (user != null && model.NewPassword != model.OldPassword)
                {
                    user.Password = model.NewPassword;
                    _usersRepo.UpdateUser(user);
                    msg = "Пароль успешно изменён";
                }
                else msg = "Ошибка. Проверьте правильность введённых данных";
            }
            else msg = "Ошибка. Проверьте, все ли поля формы вы заполнили";
            return RedirectToAction(actionName: "Index", controllerName: "Manage", new { msg });
        }
    }
}