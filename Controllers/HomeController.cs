﻿using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly DBContent _dbContent;
        public HomeController(IHubContext<ChatHub> hub, DBContent content)
        {
            _hub = hub; _dbContent = content;
        }
        public IActionResult Index(string message)
        {
            int users = _dbContent.Users.Count();
            var model = new ChatViewModel { Message = message ?? $"Привет! На данный момент в чате {users} человек" };
            return View(model);
        }
        public async Task<IActionResult> Chat(ChatViewModel model)
        {
            /*if(Regex.IsMatch(model.UserName, "_*"))
            {
                return RedirectToAction(actionName: "Index", routeValues: new { message="Имя должно состоять только из латинских символов и цифр :(" });
            }*/
            string userName = model.UserName;
            _dbContent.Users.Add(new User { Username = userName });
            _dbContent.SaveChanges();
            await _hub.Clients.All.SendAsync("MemberJoined", userName);
            return View(model);
        }
        public string GetConsoleSize(int? id)
        {
            Random r = new();
            return $"{id * r.Next(1, 4)},{id * r.Next(1, 4)}";
        }
    }
}