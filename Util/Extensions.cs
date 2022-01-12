using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using ChatApp.DB;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Util
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the required service implementation from HTTP context.
        /// </summary>
        /// <typeparam name="T">Required service type.</typeparam>
        /// <param name="hub">Hub providing its HubCallerContext property.</param>
        /// <returns></returns>
        public static T GetService<T>(this Hub hub)
            => hub.Context.GetHttpContext().RequestServices.GetRequiredService<T>();


        /// <summary>
        /// Gets the required service implementation from HTTP context.
        /// </summary>
        /// <typeparam name="T">Required service type.</typeparam>
        /// <param name="controller">Controller providing its HttpContext property.</param>
        /// <returns>The T service implementation in HTTP context request services.</returns>
        public static T GetService<T>(this Controller controller) => 
            controller.HttpContext.RequestServices.GetRequiredService<T>();

  
        /// <summary>
        /// Gets the list containing connection IDs of users who should get the notifications from hub invocations.
        /// </summary>
        /// <param name="hub">The hub that provides its HubCallerContext.</param>
        /// <param name="roomName">Name of the specific room which contains all the users who should get the notifications.</param>
        /// <returns>List of users connection IDs.</returns>
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


        /// <summary>
        /// Gets the list containing connection IDs of users who should get the notifications from hub invocations.
        /// </summary>
        /// <param name="controller">The controller that provides its HttpContext.</param>
        /// <param name="roomName">Name of the specific room which contains all the users who should get the notifications.</param>
        /// <returns>List of users connection IDs.</returns>
        public static List<string> GetIds(this Controller controller, string roomName)
        {
            List<string> list = new();
            var roomsRepo = controller.GetService<IRoomsRepository>();
            var room = roomsRepo.GetRoom(roomName);
            list = room.RoomUsers.Select(u => u.ConnectionId).ToList();
            return list;
        }


        /// <summary>
        /// Authenticates a user using cookie authentication scheme within a SignalR hub.
        /// </summary>
        /// <param name="hub">Hub providing its HubCallerContext property.</param>
        /// <param name="userName">Authenticating user's name.</param>
        /// <returns>Object that represents the current task.</returns>
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

        
        /// <summary>
        /// Authenticates a user using cookie authentication scheme within a controller.
        /// </summary>
        /// <param name="controller">Controller providing its HttpContext property.</param>
        /// <param name="userName">Authenticating user's name.</param>
        /// <returns>Object that represents the current task.</returns>
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

        public static bool CompareHashes(this object obj, string password, byte[] salt, string hashedPassword)
        {
            password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password:password,
                salt:salt,
                prf:KeyDerivationPrf.HMACSHA256,
                iterationCount:100000,
                numBytesRequested: 256/8));
            return password == hashedPassword;
        }
        public static bool ContainsUser(this Room room, string userName)
            => room.RoomUsers.FirstOrDefault(u => u.UserName == userName) != null;

        public static bool ContainsAdmin(this Room room, string userName)
            => room.Admins.FirstOrDefault(u => u.UserName == userName) != null;

        public static bool ContainsBanned(this Room room, string userName)
            => room.BannedUsers.FirstOrDefault(u => u.UserName == userName) != null;
    }
}