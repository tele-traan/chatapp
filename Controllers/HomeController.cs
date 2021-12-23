﻿using ChatApp.DB;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly DBContent _dbContent;
        public HomeController(DBContent content)
        {
            _dbContent = content;
        }
        public IActionResult Index(string message)
        {
            int users = _dbContent.GlobalChatUsers.Count();
            int rusers = _dbContent.RoomUsers.Count();
            var model = new BaseViewModel { Message = message ?? 
                $"На данный момент в основном чате {users} человек, в комнатах {rusers} человек" };
            return View(model);
        }
        public IActionResult Chat()
        {
            ISession session = HttpContext.Session;
            string userName = session.GetString("UserName");
            bool condition = userName != null
                && _dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName) != null;
            if (!condition) return RedirectToAction(actionName:"Login", controllerName:"Auth");
            return View();
        }
    }
}