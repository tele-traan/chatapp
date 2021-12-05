using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.DB;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewMessageRequest(string message, string sender)
        {
            var time = System.DateTime.Now.ToShortTimeString();
            await Clients.All.SendAsync("NewMessage", time, sender, message);
        }
        public async Task MemberLeft(string whoLeft)
        {
            HttpContext httpContext = GetHttpContextExtensions.GetHttpContext(Context);
            DBContent dbContent =  httpContext.RequestServices.GetRequiredService<DBContent>();
            var user = dbContent.Users.FirstOrDefault(u => u.Username == whoLeft);
            dbContent.Users.Remove(user);
            dbContent.SaveChanges();
            await Clients.All.SendAsync("MemberLeft", whoLeft);
        }
    }
}