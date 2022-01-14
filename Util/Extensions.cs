using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Util
{
    public static class Extensions
    {
        public static T GetService<T>(this Hub hub)
            => hub.Context.GetHttpContext().RequestServices.GetRequiredService<T>();
        public static T GetService<T>(this Controller controller) => 
            controller.HttpContext.RequestServices.GetRequiredService<T>();
        public static List<string> GetIds(this Hub hub, string roomName)
        {
            List<string> list = new();
            var roomRepo = hub.GetService<IRoomsRepository>();
            var roomUsersRepo = hub.GetService<IRoomUsersRepository>();
            var room = roomRepo.GetRoom(roomName);
            list = roomUsersRepo
                .GetAllUsers()
                .Where(u => u.Room.Name == roomName)
                .Select(u => u.ConnectionId)
                .ToList();
            return list;
        }
        public static List<string> GetIds(this Controller controller, string roomName)
        {
            List<string> list = new();
            var roomsRepo = controller.GetService<IRoomsRepository>();
            var room = roomsRepo.GetRoom(roomName);
            list = room.RoomUsers.Select(u => u.ConnectionId).ToList();
            return list;
        }
        public static Task Authenticate(this Hub hub, string userName)
        {
            var httpContext = hub.Context.GetHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return httpContext.
                SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public static Task Authenticate(this Controller controller, string userName)
        {
            var httpContext = controller.HttpContext;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var identity = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
        public static ContentResult RedirectToPostAction(this Controller controller,
            string actionName,
            string controllerName, Dictionary<string, string> parameters)
        {
            StringBuilder sb = new();
            sb.Append("<html><head><meta charset=\"utf-8\"></head>");
            sb.Append("<body>");
            sb.Append("<h1>Переадресация...</h1>");
            sb.Append($"<form name=\"f\" action=\"/{controllerName}/{actionName}\" method=\"post\">");

            foreach(var p in parameters)
                sb.Append($"<input type=\"hidden\" name=\"{p.Key}\" value=\"{p.Value}\" />");

            sb.Append("</form>");
            sb.Append("<script>");
            sb.Append("document.forms['f'].submit();");
            sb.Append("</script>");
            sb.Append("</body></html>");
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = 200,
                Content = sb.ToString()
            };
        }
        public static string GetHash(this object obj, string password, byte[] salt)
            => Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password:password,
                salt:salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount:100000,
                numBytesRequested:256/8
                ));
        public static bool ContainsUser(this Room room, string userName)
            => room.RoomUsers.FirstOrDefault(u => u.UserName == userName) is not null;
        public static bool ContainsAdmin(this Room room, string userName)
            => room.Admins.FirstOrDefault(u => u.UserName == userName) is not null;
        public static bool ContainsBanned(this Room room, string userName)
            => room.BannedUsers.FirstOrDefault(u => u.UserName == userName) is not null;
    }
}