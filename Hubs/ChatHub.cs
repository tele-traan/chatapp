using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.DB;
using System.Threading.Tasks;
using System.Linq;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewMessageRequest(string message, string sender)
        { 
            var time = System.DateTime.Now.ToString();
            await Clients.All.SendAsync("NewMessage", time, sender, message);
        }

        public async Task MemberLeft(string whoLeft)
        {
            var dbContent
                = GetHttpContextExtensions.GetHttpContext(Context).RequestServices.GetService<DBContent>();
            await Clients.All.SendAsync("MemberLeft", whoLeft);
            dbContent.Users.Remove(dbContent.Users.FirstOrDefault(u => u.Username == whoLeft));
            dbContent.SaveChanges();
        }
    }
}
