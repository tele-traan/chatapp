using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Util;
using ChatApp.DB;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatApp.Hubs
{
    public class RoomHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            ISession session = Context.GetHttpContext().Session;
            string userName = session.GetString("UserName");
            string roomName = session.GetString("RoomName");
            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            if (condition)
            {
                var dbContent = Context.GetHttpContext().GetDbContent();
                var room = dbContent.Rooms.Include(r => r.Users).FirstOrDefault(r => r.Name == roomName);
                var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
                user.RoomUser.ConnectionId = Context.ConnectionId;
                dbContent.SaveChanges();
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberJoined", userName);   
            }
            else //вчера остановился здесь - roomName всегда null и выкидывает ошибку
            {
                await Clients.Caller.SendAsync("ErrorLogging", "Ошибка при входе в комнату. Попробуйте ещё раз");
            }
            await base.OnConnectedAsync();  
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ISession session = Context.GetHttpContext().Session;
            string userName = session.GetString("UserName");
            string roomName = session.GetString("RoomName");
            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            if (condition)
            {
                var dbContent = Context.GetHttpContext().GetDbContent();
                var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
                user.RoomUser = null;
                dbContent.SaveChanges();
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberLeft", userName);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            ISession session = Context.GetHttpContext().Session;
            string userName = session.GetString("UserName");
            string roomName = session.GetString("RoomName");
            string time = DateTime.Now.ToShortTimeString();
            List<string> ids = this.GetIds(roomName);
            await Clients.Clients(ids).SendAsync("NewMessage", time, userName, message.Trim());
        }
        public async Task RoomDeleted()
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            ISession session = httpContext.Session;
            bool condition = session.GetString("IsAdmin") == "true";
            if (condition)
            {
                string userName = session.GetString("UserName");
                string roomName = session.GetString("RoomName");

                var rooms = dbContent.Rooms;
                var room = rooms.FirstOrDefault(r => r.Name == roomName);

                List<string> ids = this.GetIds(roomName);
                await Clients.Clients(ids).SendAsync("RoomDeleted");

                rooms.Remove(room);
                dbContent.SaveChanges();
            }
        }
    }
}